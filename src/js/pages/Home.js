function initMap() {
    // Lokasi awal (contoh: Jakarta)
    const jakarta = {  lat: -6.9175, lng: 107.6191 };

    // Inisialisasi peta
    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 9,
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
    Highcharts.chart('chartTMA', {
        chart: {
            type: 'line'
        },
        title: {
            text: ''
        },
        xAxis: {
            categories: [
                '01:00', '02:00', '03:00', '04:00', '05:00', '06:00',
                '07:00', '08:00', '09:00', '10:00'
            ],
            title: {
                text: 'Jam'
            }
        },
        yAxis: {
            title: {
                text: 'TMA (m)'
            },
            plotLines: [{
                value: 135.0,
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
            data: [134.0, 134.1, 134.2, 134.4, 134.3, 134.6, 134.8, 134.9, 135.1, 135.3],
            color: '#007bff'
        }],
            credits: {
            enabled: false
        }
    });

    loadAwlrPanel();
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