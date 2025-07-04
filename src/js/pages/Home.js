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
});