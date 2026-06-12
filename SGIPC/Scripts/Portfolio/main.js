document.addEventListener('DOMContentLoaded', function() {
    
    // Set footer year
    var yearEl = document.getElementById('year');
    if (yearEl) {
        yearEl.textContent = new Date().getFullYear();
    }

    // ACHIEVEMENTS CAROUSEL
    // Get elements
    var achTrack = document.getElementById('achTrack');
    var achDotsEl = document.getElementById('achDots');
    var achNextBtn = document.getElementById('achNext');
    var achPrevBtn = document.getElementById('achPrev');

    // Check if all elements exist
    if (achTrack && achDotsEl && achNextBtn && achPrevBtn) {
        console.log('Initializing achievements carousel...');
        
        var achCards = achTrack.querySelectorAll('.ach-card');
        console.log('Found ' + achCards.length + ' achievement cards');
        
        // Get actual width of first card
        var ACH_W = achCards.length > 0 ? achCards[0].offsetWidth : 480;
        console.log('Card width: ' + ACH_W);
        
        var achIdx = 0;
        var achTimer;

        // Add transition to track
        achTrack.style.transition = 'transform 0.5s ease-in-out';

        // Build dots dynamically
        for (var i = 0; i < achCards.length; i++) {
            var dot = document.createElement('div');
            dot.className = (i === 0) ? 'ach-dot active' : 'ach-dot';
            dot.style.cursor = 'pointer';
            
            // Closure to capture index
            dot.onclick = (function(dotIndex) {
                return function() {
                    console.log('Dot clicked: ' + dotIndex);
                    stopAchAuto();
                    goAch(dotIndex);
                    startAchAuto();
                };
            })(i);
            
            achDotsEl.appendChild(dot);
        }

        // Go to specific slide
        function goAch(i) {
            console.log('Going to slide: ' + i + ', total slides: ' + achCards.length);
            
            if (i >= achCards.length) {
                achIdx = 0;
            } else if (i < 0) {
                achIdx = achCards.length - 1;
            } else {
                achIdx = i;
            }

            var translateX = -(achIdx * ACH_W);
            console.log('Transform X: ' + translateX);
            achTrack.style.transform = 'translateX(' + translateX + 'px)';

            // Update dots
            var dots = achDotsEl.querySelectorAll('.ach-dot');
            for (var j = 0; j < dots.length; j++) {
                if (j === achIdx) {
                    dots[j].classList.add('active');
                } else {
                    dots[j].classList.remove('active');
                }
            }
        }

        // Next slide
        function achNext() {
            goAch(achIdx + 1);
        }

        // Previous slide
        function achPrev() {
            goAch(achIdx - 1);
        }

        // Start auto-play
        function startAchAuto() {
            achTimer = setInterval(function() {
                achNext();
            }, 4000);
        }

        // Stop auto-play
        function stopAchAuto() {
            clearInterval(achTimer);
        }

        // Next button
        achNextBtn.onclick = function() {
            console.log('Next button clicked');
            stopAchAuto();
            achNext();
            startAchAuto();
        };

        // Previous button
        achPrevBtn.onclick = function() {
            console.log('Prev button clicked');
            stopAchAuto();
            achPrev();
            startAchAuto();
        };

        // Handle window resize
        window.addEventListener('resize', function() {
            ACH_W = achCards[0].offsetWidth;
            achTrack.style.transform = 'translateX(-' + (achIdx * ACH_W) + 'px)';
        });

        // Start the carousel
        startAchAuto();
        console.log('Achievements carousel initialized successfully');
    } else {
        console.log('Carousel elements not found');
    }

    // COMMITTEE CAROUSEL 
    var track = document.getElementById('track');
    var dotsEl = document.getElementById('dots');
    var nextBtn = document.getElementById('next');
    var prevBtn = document.getElementById('prev');

    if (track && dotsEl && nextBtn && prevBtn) {
        var cards = track.querySelectorAll('.leader-card');
        var CARD_W = cards.length > 0 ? cards[0].offsetWidth : 300;
        var idx = 0;
        var timer;

        track.style.transition = 'transform 0.5s ease';

        // Build dots
        for (var i = 0; i < cards.length; i++) {
            var dot = document.createElement('div');
            dot.className = (i === 0) ? 'dot active' : 'dot';
            dot.style.cursor = 'pointer';
            
            dot.onclick = (function(dotIndex) {
                return function() {
                    stopAuto();
                    goToSlide(dotIndex);
                    startAuto();
                };
            })(i);
            
            dotsEl.appendChild(dot);
        }

        // Go to slide
        function goToSlide(i) {
            if (i >= cards.length) {
                idx = 0;
            } else if (i < 0) {
                idx = cards.length - 1;
            } else {
                idx = i;
            }

            track.style.transform = 'translateX(-' + (idx * CARD_W) + 'px)';

            var dots = dotsEl.querySelectorAll('.dot');
            for (var j = 0; j < dots.length; j++) {
                if (j === idx) {
                    dots[j].classList.add('active');
                } else {
                    dots[j].classList.remove('active');
                }
            }
        }

        // Navigation
        function goNext() {
            goToSlide(idx + 1);
        }

        function goPrev() {
            goToSlide(idx - 1);
        }

        // Auto-play
        function startAuto() {
            timer = setInterval(function() {
                goNext();
            }, 3000);
        }

        function stopAuto() {
            clearInterval(timer);
        }

        // Button handlers
        nextBtn.onclick = function() {
            stopAuto();
            goNext();
            startAuto();
        };

        prevBtn.onclick = function() {
            stopAuto();
            goPrev();
            startAuto();
        };

        // Resize handler
        window.addEventListener('resize', function() {
            CARD_W = cards[0].offsetWidth;
            track.style.transform = 'translateX(-' + (idx * CARD_W) + 'px)';
        });

        startAuto();
    }

    // HERO TYPING EFFECT 
    var heroTyped  = document.getElementById('hero-typed');
    var heroCursor = document.querySelector('.hero-cursor');
    var heroSub    = document.getElementById('hero-sub');
    var heroBtns   = document.getElementById('hero-btns');

    if (heroTyped) {
        var heroText = "Special Group Interested in Programming Contest";
        var heroIdx  = 0;

        function typeHero() {
            if (heroIdx < heroText.length) {
                heroTyped.textContent += heroText[heroIdx++];
                setTimeout(typeHero, 42);
            } else {
                // Hide cursor after typing done
                if (heroCursor) heroCursor.style.display = 'none';
                // Fade in subtitle
                if (heroSub) heroSub.classList.add('show');
                // Fade in buttons after short delay
                setTimeout(function () {
                    if (heroBtns) heroBtns.classList.add('show');
                }, 400);
            }
        }

        typeHero();
    }
    // SCROLL ANIMATIONS 
    var fadeEls = document.querySelectorAll('.fade-up');

    if ('IntersectionObserver' in window) {
        var observer = new IntersectionObserver(function(entries) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    observer.unobserve(entry.target); // animate once
                }
            });
        }, { threshold: 0.15 });

        fadeEls.forEach(function(el) {
            observer.observe(el);
        });
    } else {
        fadeEls.forEach(function(el) {
            el.classList.add('visible');
        });
    }

});