// Character counter for the textarea (ReasonForJoin field)
document.addEventListener('DOMContentLoaded', function() {
    var reasonInput = document.getElementById('ReasonForJoin');
    var charCount = document.getElementById('charCount');

    if (reasonInput && charCount) {
        reasonInput.addEventListener('input', function() {
            charCount.textContent = this.value.length;
        });
    }

    // Handle form submission - show success message
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            // Check if form is valid by validating with jQuery
            if ($(form).valid()) {
                // Show success message
                var successMsg = document.getElementById('successMessage');
                if (successMsg) {
                    successMsg.style.display = 'block';
                    // Disable submit button to prevent duplicate submissions
                    var submitBtn = document.getElementById('submitBtn');
                    if (submitBtn) {
                        submitBtn.disabled = true;
                        submitBtn.textContent = 'Submitting...';
                    }
                }
            }
        });
    }
});

// jQuery Validation customization
$(document).ready(function() {
    // Customize error styling for form validation
    $.validator.setDefaults({
        errorClass: "err show",
        validClass: "",
        errorElement: "div",
        highlight: function(element, errorClass, validClass) {
            $(element).addClass('input-validation-error');
        },
        unhighlight: function(element, errorClass, validClass) {
            $(element).removeClass('input-validation-error');
        }
    });
});
    var re  = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!val || !re.test(val)) {
        showError('email');
        return false;
    }
    hideError('email');
    return true;



function handleSubmit(e) {
    e.preventDefault();
    var valid = true;

    if (!validateField('fullname')) valid = false;
    if (!validateEmailField())      valid = false;
    if (!validateField('roll'))     valid = false;
    if (!validateField('dept'))     valid = false;
    if (!validateField('batch'))    valid = false;
    if (!validateField('reason'))   valid = false;

    if (!valid) return;

    var applicationData = {
        status:    'pending', // admin will change this to 'approved' or 'rejected'
        submittedAt: new Date().toISOString(),
        personal: {
            fullname: document.getElementById('fullname').value.trim(),
            email:    document.getElementById('email').value.trim(),
            roll:     document.getElementById('roll').value.trim(),
            dept:     document.getElementById('dept').value,
            batch:    document.getElementById('batch').value
        },
        handles: {
            codeforces: document.getElementById('cf').value.trim() || null,
            atcoder:    document.getElementById('ac').value.trim() || null,
            codechef:   document.getElementById('cc').value.trim() || null,
            leetcode:   document.getElementById('lc').value.trim() || null,
            vjudge:     document.getElementById('vj').value.trim() || null
        },
        reason: document.getElementById('reason').value.trim()
    };

    showSuccess();
}

function showSuccess() {
    document.getElementById('applyForm').style.display   = 'none';
    document.querySelector('.form-header').style.display = 'none';
    document.getElementById('successBox').classList.add('show');
}