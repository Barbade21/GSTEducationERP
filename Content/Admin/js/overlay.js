// Function to display the loading overlay and simulate progress
function showLoadingOverlay() {
    var progress = 0;
    $('#progress-bar').css('width', '0%');
    $('#progress-percentage').text('0%');
    $('#loading-overlay').css('opacity', '1').show();

    var interval = setInterval(function () {
        if (progress < 100) {
            progress += Math.floor(Math.random() * 5) + 2;
            if (progress > 100) progress = 100;
        }

        $('#progress-bar').css('width', progress + '%');
        $('#progress-percentage').text(progress + '%');

        if (progress >= 100) {
            clearInterval(interval);
            $('#progress-bar').css('width', '100%');
            $('#progress-percentage').text('100%');
        }
    }, 300);

    return interval;
}

// Function to finalize and hide the loading overlay
function hideLoadingOverlay(interval) {
    $('#progress-bar').css('width', '100%');
    $('#progress-percentage').text('100%');
    $('#loading-overlay').fadeOut(500, function () {
        $(this).hide();
    });
    clearInterval(interval);
}