moment.locale('id');
const lastReadingDateRaw = getLastReadingDate().lastReading;
const LAST_READING_DATE = lastReadingDateRaw ? moment(lastReadingDateRaw).format("DD MMMM YYYY") : moment().format("DD MMMM YYYY");
const LAST_READING_DATE_RAW = lastReadingDateRaw  ? moment(lastReadingDateRaw).format("YYYY-MM-DD") : moment().format("YYYY-MM-DD");

$(document).ready(function() {
    // Set placeholder input dengan format tanggal sekarang
    document.getElementById("periode").placeholder = LAST_READING_DATE;
    document.getElementById("periode-end").placeholder = LAST_READING_DATE;
    document.getElementById("periode-grafik").placeholder = LAST_READING_DATE;
    document.getElementById("periode-grafik-end").placeholder = LAST_READING_DATE;

    //init flatpickr
    flatpickr("#periode", {
        dateFormat: "Y-m-d",           // format nilai input
        altInput: true,                // tampilkan format alternatif
        altFormat: "d F Y",            // tampilan di UI: 23 Juli 2025
        locale: "id",
        defaultDate: LAST_READING_DATE_RAW,
        maxDate: "today",
        disableMobile: true,
    });

    flatpickr("#periode-end", {
        dateFormat: "Y-m-d",
        altInput: true,
        altFormat: "d F Y",
        locale: "id",
        defaultDate: LAST_READING_DATE_RAW,
        maxDate: "today",
        disableMobile: true,
    });

    flatpickr("#periode-grafik", {
        dateFormat: "Y-m-d",           // format nilai input
        altInput: true,                // tampilkan format alternatif
        altFormat: "d F Y",            // tampilan di UI: 23 Juli 2025
        locale: "id",
        defaultDate: LAST_READING_DATE_RAW,
        maxDate: "today",
        disableMobile: true,
    });

    flatpickr("#periode-grafik-end", {
        dateFormat: "Y-m-d",
        altInput: true,
        altFormat: "d F Y",
        locale: "id",
        defaultDate: LAST_READING_DATE_RAW,
        maxDate: "today",
        disableMobile: true,
    });

    //ambil value flacpikr
    const periode = $("#periode").val();
    const periodEnd = $("#periode-end").val();
    const periodeGrafik = $("#periode-grafik").val();
    const periodeGrafikEnd = $("#periode-grafik-end").val();

    //init table dan grafik
    generateDataTable(periode, periodEnd, function () {
        renderChartFromTable(periodeGrafik, periodeGrafikEnd);
    });
    // renderChartFromTable(periodeGrafik, periodeGrafikEnd);

    //onchange datepicker data dan grafik
    $("#periode").on('change', function () {
        const value = $(this).val();
        generateDataTable(value, periodEnd);
    });

    $("#periode-end").on('change', function () {
        const value = $(this).val();
        generateDataTable(periode, value);
    });

    $("#periode-grafik").on('change', function () {
        const start = $(this).val();
        const end = $("#periode-grafik-end").val();

        generateDataTable(start, end, function () {
            renderChartFromTable(start, end);
        });
    });

    $("#periode-grafik-end").on('change', function () {
        const start = $("#periode-grafik").val();
        const end = $(this).val();

        generateDataTable(start, end, function () {
            renderChartFromTable(start, end);
        });
    });
});

function generateDataTable(periode, periodEnd, callback) {
    let COLUMNS = [
        {
            title: "Tanggal Baca",
            data: "readingAt",
            class: "text-center",
            render: function (data) {
                // return moment(data).format("DD MMMM YYYY HH:mm");
                return moment(data).locale('id').format("DD MMMM YYYY");
            }
        },
        {
            title: "Jam",
            data: "readingAt",
            class: "text-center",
            render: function(data) {
                return moment.parseZone(data).format("HH:mm");
            }
        },
        {
            title: "Tinggi Muka Air (m)",
            data: "waterLevel",
            class: "text-center",
            render: function (data) {
                return `${data.toFixed(2)} m`;
            }
        }
    ];

    if ($.fn.DataTable.isDataTable("#table-telemetri")) {
        $("#table-telemetri").DataTable().clear().destroy();
    }

    $("#table-telemetri").DataTable({
        dom: "lrtip",
        scrollY: "400px",
        processing: true,
        serverSide: false,
        scrollCollapse: true,
        paging: true,
        ajax: {
            url: `/Home/GetDataTableAwlr?periode=${periode}&periodEnd=${periodEnd}`,
            type: "GET",
            dataSrc: ""
        },
        columns: COLUMNS,
        responsive: true,
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.4/i18n/id.json"
        },
        order: [[0, "desc"]],
        pageLength: 10,
        initComplete: function () {
            if (callback && typeof callback === "function") {
                callback();
            }
        }
    });
}

function renderChartFromTable(periode, periodEnd) {
    const start = moment(periode);
    const end = moment(periodEnd).endOf('day');

    let dataTable = $('#table-telemetri').DataTable();
    let data = dataTable.rows().data().toArray();

    let chartData = [];
    data.forEach(item => {
        let tgl = moment.parseZone(item.readingAt); // Gunakan parseZone
        if (tgl.isBetween(start, end, null, '[]')) {
            chartData.push([tgl.valueOf(), parseFloat(item.waterLevel)]);
        }
    });

    chartData.sort((a, b) => a[0] - b[0]);
    let subtitle;
    if(periode == periodEnd) {
        subtitle = `Periode: ${moment(periode).locale('id').format("DD MMMM YYYY")}`;
    } else {
        subtitle = `Periode: ${moment(periode).locale('id').format("DD MMMM YYYY")} - ${moment(periodEnd).locale('id').format("DD MMMM YYYY")}`;
    }

    Highcharts.stockChart("container", {
        chart: {
            type: "areaspline",
            zoomType: "x",
            spacingTop: 20,
            spacingBottom: 20,
            scrollablePlotArea: {
                minWidth: 1200, // Grafik bisa discroll jika terlalu panjang
                scrollPositionX: 0
            }
        },
        navigation: {
            buttonOptions: {
                enabled: false, // Matikan tombol zoom/pan
            }
        },
        rangeSelector: {
            enabled: false
        },
        title: {
            text: "Grafik Tinggi Muka Air",
            style: {
                fontSize: "16px",
                fontWeight: "bold"
            }
        },
        subtitle: {
            text: subtitle,
            style: {
                fontSize: "13px",
                color: "#666"
            }
        },
        xAxis: {
            type: "datetime",
            title: { text: "Tanggal" },
            labels: {
                format: "{value:%d %b %Y %H:%M}",
                rotation: -45,
                align: "right"
            }
        },
        yAxis: {
            title: {
                text: 'Tinggi Muka Air (cm)'
            },
            opposite: false, // pastikan axis di sebelah kiri
            labels: {
                format: '{value} cm',
                style: {
                    fontSize: '12px'
                }
            },
            gridLineDashStyle: 'Dash',
            lineWidth: 1,
            lineColor: '#999',
            tickColor: '#999'
        },
        tooltip: {
            shared: true,
            useHTML: true,
            formatter: function () {
                let s = `<b>${Highcharts.dateFormat('%A, %d %B %Y %H:%M', this.x)}</b><br/>`;
                this.points.forEach(point => {
                    s += `<span style="color:${point.color}">\u25CF</span> ${point.series.name}: <b>${point.y.toFixed(2)} cm</b><br/>`;
                });
                return s;
            }
        },
        plotOptions: {
            series: {
                marker: {
                    enabled: true,
                    radius: 4
                },
                lineWidth: 2,
                states: {
                    hover: {
                        lineWidth: 3
                    }
                }
            },
            areaspline: {
                fillOpacity: 0.3
            }
        },
        series: [{
            name: "TMA",
            data: chartData.map(x => [new Date(x[0]).getTime(), x[1]])
        }],
        credits: { enabled: false },
        exporting: { enabled: true }
    });
}

function getLastReadingDate() {
    let result;
    $.ajax({
    url: '/Home/GetLastReadingDate?deviceId=MAA1',
    type: 'GET',
    async: false, // This makes the request synchronous
        success: function (data) {
        const currentDateTime = moment.parseZone(data.lastReading);
        const hour = currentDateTime.hour();
        result = currentDateTime.format("YYYY-MM-DD");
    },
    error: function(jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
            result = null;
        }
    });

    return result;
}