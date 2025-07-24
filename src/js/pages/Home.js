let map;
function initMap() {
    // Lokasi awal (contoh: Jakarta)
    const jakarta = {  lat: -6.9175, lng: 107.6191 };

    // Inisialisasi peta
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 10,
        center: jakarta,
        gestureHandling: 'greedy', // Mengizinkan zoom menggunakan scroll
        mapTypeId: google.maps.MapTypeId.HYBRID,
        // mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
            position: google.maps.ControlPosition.BOTTOM_LEFT // Pindahkan ke bawah kiri
        },
        styles: [
            {
                featureType: "administrative.locality", // Nama kota
                elementType: "labels",
                stylers: [{ visibility: "on" }]
            },
            {
                featureType: "administrative.neighborhood", // Nama kelurahan
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "poi", // Tempat umum
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "road",
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "transit",
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "administrative.province", // Nama provinsi
                elementType: "labels",
                stylers: [{ visibility: "on" }]
            }
        ]
    });

    const geocoder = new google.maps.Geocoder();
    geocoder.geocode({ location: jakarta }, (results, status) => {
        if (status === "OK" && results[0]) {
            let province = '';
            for (const component of results[0].address_components) {
                if (component.types.includes("administrative_area_level_1")) {
                    province = component.long_name;
                    break;
                }
            }

            const infowindow = new google.maps.InfoWindow({
                content: `<strong>${province}</strong>`,
            });
            infowindow.open(map);
        }
    });
}

$(document).ready(function() {
    loadAwlrPanel();
    loadChartTMA();
    loadAwlrLastReading();
});

async function loadAwlrPanel() {
    try {
        const res = await fetch('/Home/GetLatestReading');
        if (!res.ok) throw new Error("Gagal memuat data");

        const data = await res.json();

        const html = `<div class="col-md-4 mb-3">
                <h5>Nama Pos:</h5>
                <p class="mb-1 fw-semibold">${data.name || '-'}</p>
            </div>
            <div class="col-md-4 mb-3">
                <h5>Device:</h5>
                <p class="mb-1 fw-semibold">${data.brand_name || '-'} <span class="badge bg-warning text-dark">${data.device_id || '-'}</span></p>
            </div>
            <div class="col-md-4 mb-3">
                <h5>Tinggi Muka Air (TMA):</h5>
                <p class="mb-1 text-primary fw-semibold">${parseFloat(data.water_level).toFixed(2)} cm</p>
            </div>
            <div class="col-md-6 mb-3">
                <h5>Status:</h5>
                <span class="badge ${getStatusBadgeClass(data.warning_status)} text-dark">${data.warning_status || '-'}</span>
            </div>
            <div class="col-md-6 mb-2">
                <h5>Waktu Pengukuran:</h5>
                <p class="mb-0 text-muted">${formatDate(data.reading_at)}</p>
            </div>`;

        document.getElementById("info-panel").innerHTML = html;
    } catch (err) {
        console.error("Error:", err);
        document.getElementById("info-panel").innerHTML = `<p class="text-danger">Gagal memuat data.</p>`;
    }
}

async function loadChartTMA() {
    try {
        const response = await fetch('/Home/GetLastGrafik');
        const result = await response.json();

        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

        const data = result
            .filter(item => {
                const readingDate = new Date(item.reading_at);
                return readingDate >= today && readingDate <= now;
            })
            .sort((a, b) => new Date(a.reading_at) - new Date(b.reading_at));

        if (data.length === 0) {
            console.warn("Tidak ada data hari ini.");
            return;
        }

        const categories = data.map(item => {
            const date = new Date(item.reading_at);
            return date.toLocaleTimeString('id-ID', { hour: '2-digit', minute: '2-digit' });
        });

        const waterLevels = data.map(item => parseFloat(item.water_level));
        const siagaValue = data[0].siaga1 != null ? parseFloat(data[0].siaga1) : 135.0;

        Highcharts.chart('chartTMA', {
            chart: { type: 'line' },
            title: { text: '' },
            xAxis: {
                categories: categories,
                title: { text: 'Jam' }
            },
            yAxis: {
                title: { text: 'TMA (m)' },
                min: Math.min(...waterLevels, siagaValue) - 0.2,
                max: Math.max(...waterLevels, siagaValue) + 0.2,
                plotLines: [{
                    value: siagaValue,
                    color: 'orange',
                    dashStyle: 'ShortDash',
                    width: 2,
                    label: {
                        text: 'Batas Siaga',
                        align: 'right',
                        style: { color: 'orange', fontWeight: 'bold' }
                    }
                }]
            },
            series: [{
                name: 'TMA',
                data: waterLevels,
                color: '#007bff'
            }],
            credits: { enabled: false }
        });

    } catch (error) {
        console.error('Gagal memuat grafik TMA:', error);
    }
}

function loadAwlrLastReading() {
    getData(`/Home/GetLastReadingMap`).then(res => {
        const result = res;

        if (result && result.data.metaData?.code === 200) {
            $.each(result.data.response, function (key, awlr_last_reading) {
                // Pastikan koordinat valid
                if (awlr_last_reading.latitude && awlr_last_reading.longitude) {
                    const marker = new google.maps.Marker({
                        position: {
                            lat: parseFloat(awlr_last_reading.latitude),
                            lng: parseFloat(awlr_last_reading.longitude)
                        },
                        map: map,
                        title: awlr_last_reading.name,
                        icon: {
                            url: 'img/duga.png',
                            scaledSize: new google.maps.Size(40, 50),
                            anchor: new google.maps.Point(16, 32)
                        }
                    });

                    // Progress bar siaga
                    let warning_status_bar = `
                        <div class="progress-meter mt-2 mb-2">
                            <div class="meter meter-normal meter-left" style="width: 25%;" title="Normal <br> < 6 m">
                                <span class="fw-normal meter-text" style="color: #4DC27E;">0</span>
                            </div>
                            <div class="meter meter-waspada meter-left" style="width: 25%;" title="Siaga 3 <br> ${awlr_last_reading.siaga3 || 0} cm - ${awlr_last_reading.siaga2 || 0} cm">
                                <span class="fw-normal meter-text" style="color: #FFDA4F;">${awlr_last_reading.siaga3 || 0} cm</span>
                            </div>
                            <div class="meter meter-siaga meter-left" style="width: 25%;" title="Siaga 2 <br> ${awlr_last_reading.siaga2 || 0} cm - ${awlr_last_reading.siaga1 || 0} cm">
                                <span class="fw-normal meter-text" style="color: #FFA600;">${awlr_last_reading.siaga2 || 0} cm</span>
                            </div>
                            <div class="meter meter-awas meter-left" style="width: 25%;" title="Siaga 1 <br> > ${awlr_last_reading.siaga1 || 0} cm">
                                <span class="fw-normal meter-text" style="color: #EF5350;">${awlr_last_reading.siaga1 || 0} cm</span>
                            </div>
                        </div>
                    `;

                    let status;
                    if(awlr_last_reading.status == "Normal") {
                        status = "bg-success";
                    } else if(awlr_last_reading.status == "Siaga 3") {
                        status = "bg-warning";
                    } else if(awlr_last_reading.status == "Siaga 2") {
                        status = "bg-orange";
                    } else if(awlr_last_reading.status == "Siaga 1") {
                        status = "bg-danger"
                    }

                    // Konten InfoWindow
                    const contentHtml = `
                        <div style="font-family: 'Arial', sans-serif;
                                    max-width: 350px;
                                    font-size: 13px;
                                    text-align: left;
                                    padding: 0;
                                    margin: 0;
                                    line-height: 1.4;">

                            <!-- Judul -->
                            <div style="font-size: 16px;
                                        font-weight: bold;
                                        text-align: left;
                                        margin: 0 0 6px 0;">
                                ${awlr_last_reading.name || 'Tanpa Nama'}
                            </div>

                            <!-- Tabel Info -->
                            <table style="width: 100%; border-collapse: collapse; margin-bottom: 8px;">
                                <tr>
                                    <td style="color: #555;">Device</td>
                                    <td style="text-align: right;">
                                        <div style="display: flex; justify-content: flex-end; align-items: center; gap: 6px;">
                                            <span>${awlr_last_reading.brand_name || '-'}</span>
                                            <span style="
                                                display: inline-block;
                                                background-color: #ffc107;
                                                color: #000;
                                                font-size: 11px;
                                                padding: 2px 6px;
                                                border-radius: 6px;
                                            ">
                                                ${awlr_last_reading.device_id || '-'}
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="color: #555;">Koordinat</td>
                                    <td style="text-align: right; padding-left: 8px;">
                                        ${parseFloat(awlr_last_reading.latitude).toFixed(6)}, ${parseFloat(awlr_last_reading.longitude).toFixed(6)}
                                    </td>
                                </tr>
                                <tr>
                                    <td style="color: #555;">Status</td>
                                    <td style="text-align: right;">
                                        <span style="
                                            display: inline-block;
                                            padding: 2px 6px;
                                            font-size: 12px;
                                            border-radius: 6px;
                                            color: white;"
                                            class="${status}">
                                            ${awlr_last_reading.status}
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="color: #555;">TMA</td>
                                    <td style="text-align: right; font-weight: bold; font-size: 14px;">
                                        ${parseFloat(awlr_last_reading.water_level || 0).toFixed(1)} cm
                                    </td>
                                </tr>
                            </table>

                            <!-- Progress Meter -->
                            ${warning_status_bar}<br>

                            <!-- Waktu -->
                            <div style="display: flex; align-items: center; font-size: 12px; color: #555; margin-top: 4px;">
                                <i class="far fa-clock" style="margin-right: 6px;"></i>
                                ${moment(awlr_last_reading.reading_at).locale('id').format('YYYY-MM-DD HH:mm')}
                            </div>

                            <div style="text-align: center; margin-top: 10px;">
                                <a href="/Home/StationDetail" class="btn btn-info btn-sm" style="display: block; width: 100%; border-radius: 25px;">Lihat Detail</a>
                            </div>
                        </div>
                    `;

                    // Buat InfoWindow
                    const infowindow = new google.maps.InfoWindow({ content: contentHtml });

                    // Event klik marker
                    marker.addListener("click", () => {
                        infowindow.open(map, marker);
                    });
                }
            });

            setTimeout(adjustPadding, 100);
        }
    }).catch(err => {
        console.log(err);
    });
}


function getStatusBadgeClass(status) {
    switch ((status || '').toLowerCase()) {
        case 'normal': return 'bg-success text-dark';
        case 'siaga 1': return 'bg-danger text-dark';
        case 'siaga 2': return 'bg-orange';
        case 'siaga 3': return 'bg-warning';
        default: return 'bg-secondary';
    }
}

// Fungsi bantu: format tanggal ke format lokal
function formatDate(dateStr) {
    if (!dateStr) return '-';
    const date = new Date(dateStr);

    const tanggal = date.toLocaleDateString('id-ID', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        timeZone: 'Asia/Jakarta'
    });

    const jam = date.toLocaleTimeString('id-ID', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
        timeZone: 'Asia/Jakarta'
    }).replace('.', ':'); // Ubah titik menjadi titik dua jika perlu

    return `${tanggal} ${jam} WIB`;
}

function getData(url) {
    return fetch(url)
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        });
}

function adjustPadding() {
    const footers = document.querySelectorAll('.footer');
    const tabPanes = document.querySelectorAll('.tab-pane');

    // Ensure there are corresponding footers and tab-panes
    footers.forEach((footer, index) => {
        const tabPane = tabPanes[index];
        if (footer && tabPane) {
            const footerHeight = footer.offsetHeight;
            tabPane.style.paddingBottom = `${footerHeight}px`;
        }
    });
}