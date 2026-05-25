document.getElementById('year').textContent = new Date().getFullYear();


function checkStrength(val) {
    var bars  = [
        document.getElementById('s1'),
        document.getElementById('s2'),
        document.getElementById('s3'),
        document.getElementById('s4')
    ];
    var label  = document.getElementById('strength-label');
    var score  = 0;

    if (val.length >= 8)            score++;
    if (/[A-Z]/.test(val))          score++;
    if (/[0-9]/.test(val))          score++;
    if (/[^A-Za-z0-9]/.test(val))   score++;

    var colors = ['#f87171', '#fb923c', '#facc15', '#34d399'];
    var labels = ['Weak', 'Fair', 'Good', 'Strong'];

    bars.forEach(function(bar, i) {
        bar.style.background = i < score ? colors[score - 1] : 'var(--border)';
    });

    label.textContent = val.length ? (labels[score - 1] || '') : '';
    label.style.color = val.length ? colors[score - 1] : 'var(--muted)';
}

function handleSignup(e) {
    e.preventDefault();

    var pw   = document.getElementById('password').value;
    var conf = document.getElementById('confirm').value;
    var err  = document.getElementById('pw-err');

    if (pw !== conf) {
        err.style.display = 'block';
        return;
    }

    err.style.display = 'none';
}
