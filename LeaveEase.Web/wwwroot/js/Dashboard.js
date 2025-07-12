$(document).ready(function () {

    function loadDashboard(filter) {
        $.ajax({
            url: "/Home/DashboardData",
            data: { filter: filter },
            type: 'GET',
            success: function (result) {
                $('#dashboardContainer').html(result);
            }
        });
    }

    // Load dashboard with 'today' on page load
    loadDashboard('today');

    // Bind change event to dropdown
    $('#timeFilter').on('change', function () {
        var filter = $(this).val();
        loadDashboard(filter);
    });
           
        });