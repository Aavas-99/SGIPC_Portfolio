// Footer year
document.getElementById('year').textContent = new Date().getFullYear();

// Achievements Carousel

var achTrack  = document.getElementById('achTrack');
var achCards  = achTrack.querySelectorAll('.ach-card');
var achDotsEl = document.getElementById('achDots');
var ACH_W     = 480;   // Equal to .ach-window and .ach-card width
var achIdx    = 0;
var achTimer;

// Build dots
achCards.forEach(function(_, i) {
    var d = document.createElement('div');
    d.className = 'ach-dot' + (i === 0 ? ' active' : '');
    d.onclick = function(index) {
        return function() {
            stopAchAuto();
            goAch(index);
            startAchAuto();
        };
    }(i);
    achDotsEl.appendChild(d);
});

function goAch(i) {
    if (i >= achCards.length) {
        achIdx = 0;
    } else if (i < 0) {
        achIdx = achCards.length - 1;
    } else {
        achIdx = i;
    }

    achTrack.style.transform = 'translateX(-' + (achIdx * ACH_W) + 'px)';

    var dots = achDotsEl.querySelectorAll('.ach-dot');
    for (var j = 0; j < dots.length; j++) {
        if (j === achIdx) {
            dots[j].classList.add('active');
        } else {
            dots[j].classList.remove('active');
        }
    }
}

function achNext() { goAch(achIdx + 1); }
function achPrev() { goAch(achIdx - 1); }

function startAchAuto() { achTimer = setInterval(achNext, 4000); }
function stopAchAuto()  { clearInterval(achTimer); }

document.getElementById('achNext').onclick = function() {
    stopAchAuto(); achNext(); startAchAuto();
};

document.getElementById('achPrev').onclick = function() {
    stopAchAuto(); achPrev(); startAchAuto();
};

startAchAuto();