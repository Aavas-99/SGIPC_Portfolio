// Set current year in footer
document.getElementById('year').textContent = new Date().getFullYear();

// Character counter for the textarea
var reasonInput = document.getElementById('reason');
var charCount   = document.getElementById('charCount');

reasonInput.addEventListener('input', function() {
    var len = this.value.length;
    charCount.textContent = len;

    // Warning when approaching limit
    if (len > 500) {
        this.value = this.value.substring(0, 500);
        charCount.textContent = 500;
    }

    charCount.style.color = len >= 450 ? '#fb923c' : '';
});

function showError(id) {
    document.getElementById('err-' + id).classList.add('show');
}

function hideError(id) {
    document.getElementById('err-' + id).classList.remove('show');
}

function validateField(id) {
    var val = document.getElementById(id).value.trim();
    if (!val) {
        showError(id);
        return false;
    }
    hideError(id);
    return true;
}

['fullname', 'roll', 'dept', 'batch', 'reason'].forEach(function(id) {
    document.getElementById(id).addEventListener('input', function() {
        hideError(id);
    });
    document.getElementById(id).addEventListener('change', function() {
        hideError(id);
    });
});


function handleSubmit(e) {
    e.preventDefault();
    var valid = true;

    if (!validateField('fullname')) valid = false;
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