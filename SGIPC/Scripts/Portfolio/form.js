// Character counter + form handling
document.addEventListener('DOMContentLoaded', function () {

    // Character counter
    var reasonInput = document.getElementById('ReasonForJoin');
    var charCount = document.getElementById('charCount');

    if (reasonInput && charCount) {

        // Initial count
        charCount.textContent = reasonInput.value.length;

        reasonInput.addEventListener('input', function () {
            charCount.textContent = this.value.length;
        });
    }

    // Form submission handling
    var form = document.querySelector('form');

    if (form) {

        form.addEventListener('submit', function (e) {

            // Validate using jQuery unobtrusive validation
            if (!$(form).valid()) {
                return;
            }

            // Disable button
            var submitBtn = document.getElementById('submitBtn');

            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.textContent = 'Submitting...';
            }

            // Show success message after submit
            var successMsg = document.getElementById('successMessage');

            if (successMsg) {
                successMsg.style.display = 'block';
            }
        });
    }
});


// jQuery validation styling
$(document).ready(function () {

    $.validator.setDefaults({

        errorClass: "err show",

        errorElement: "div",

        highlight: function (element) {
            $(element).addClass('input-validation-error');
        },

        unhighlight: function (element) {
            $(element).removeClass('input-validation-error');
        }
    });

});