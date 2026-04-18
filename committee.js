// we need the following items
var track = document.getElementById('track');
var cards = track.querySelectorAll('.leader-card');
var dotsEl = document.getElementById('dots');

var CARD_W = 300;

// Current index
var idx = 0;

// for rotation
var timer;


// dot building

for (var i = 0; i < cards.length; i++) {
    var dot = document.createElement('div');

    if (i === 0) 
    {
        dot.className = 'dot active';
    } 
    else 
    {
        dot.className = 'dot';
    }

    // making dot interactive
    dot.onclick = function(clickedIndex) 
    {
        return function() 
        {
            stopAuto();
            goToSlide(clickedIndex);
            startAuto();
        };
    }(i);

    dotsEl.appendChild(dot);
}

// Move the carousel to slide number i
function goToSlide(i) {
    // first slide comes again after last slide
    if (i >= cards.length) {
        idx = 0;
    } else if (i < 0) {
        idx = cards.length - 1;
    } else {
        idx = i;
    }

    // Slide the track left by the right amount
    track.style.transform = 'translateX(-' + (idx * CARD_W) + 'px)';

    // update which dot is active
    var dots = dotsEl.querySelectorAll('.dot');
    for (var j = 0; j < dots.length; j++) 
    {
        if (j === idx) 
        {
            dots[j].classList.add('active');
        } 
        else 
        {
            dots[j].classList.remove('active');
        }
    }
}

// next slide
function goNext() 
{
    goToSlide(idx + 1);
}

// previous slide
function goPrev() 
{
    goToSlide(idx - 1);
}


// auto rotate after 3 seconds

function startAuto() {
    timer = setInterval(goNext, 3000);
}

function stopAuto() {
    clearInterval(timer);
}


// next and prev button click effects

document.getElementById('next').onclick = function() 
{
    stopAuto();
    goNext();
    startAuto();
};

document.getElementById('prev').onclick = function() 
{
    stopAuto();
    goPrev();
    startAuto();
};


// Start the auto rotation when the page loads
startAuto();