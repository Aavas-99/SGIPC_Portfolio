// Character counter for message textarea
document.addEventListener('DOMContentLoaded', function() {
    var messageInput = document.getElementById('Message');
    var charCount = document.getElementById('charCount');

    if (messageInput && charCount) {
        messageInput.addEventListener('input', function() {
            charCount.textContent = this.value.length;
        });
    }

    // jQuery Validation customization
    $.validator.setDefaults({
        errorClass: "err-msg show",
        validClass: "",
        errorElement: "div",
        highlight: function(element, errorClass, validClass) {
            $(element).addClass('input-validation-error');
        },
        unhighlight: function(element, errorClass, validClass) {
            $(element).removeClass('input-validation-error');
        }
    });

    // Handle form submission
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            if ($(form).valid()) {

                // Disable submit button
                var submitBtn = document.getElementById('submitBtn');

                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.textContent = 'Sending...';
                }
            }
        });
    }
});
