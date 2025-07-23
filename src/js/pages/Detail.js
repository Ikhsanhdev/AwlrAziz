moment.locale('id');
const lastReadingDateRaw = getLastReadingDate().lastReading;
const LAST_READING_DATE = lastReadingDateRaw ? moment(lastReadingDateRaw).format("DD MMMM YYYY") : moment().format("DD MMMM YYYY");
const LAST_READING_DATE_RAW = lastReadingDateRaw  ? moment(lastReadingDateRaw).format("YYYY-MM-DD") : moment().format("YYYY-MM-DD");

$(document).ready(function() {
    // Set placeholder input dengan format tanggal sekarang
    document.getElementById("periode").placeholder = LAST_READING_DATE;
    document.getElementById("periode-end").placeholder = LAST_READING_DATE;

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

    const periode = $("#periode").val();
    const periodEnd = $("#periode-end").val();
});

function generateDataTable(periode, periodEnd) {
    let COLUMNS = [
        {
            title: "Tanggal Baca",
            data: "readingAt",
            render: function (data) {
                return moment(data).format("DD MMMM YYYY HH:mm"); // Format sesuai kebutuhan
            }
        },
        {
            title: "Water Level (cm)",
            data: "waterLevel",
            render: function (data) {
                return `${data.toFixed(2)} cm`;
            }
        }
    ];

    if ($.fn.DataTable.isDataTable("#table-telemetri")) {
        $("#table-telemetri").DataTable().clear().destroy();
    }

    $("#table-telemetri").DataTable({
        processing: true,
        serverSide: false,
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
        pageLength: 10
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