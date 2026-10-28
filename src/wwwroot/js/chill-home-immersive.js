(() => {
  'use strict';
  const root = document.querySelector('.chill-immersive');
  if (!root) return;
  const reduced = window.matchMedia('(prefers-reduced-motion: reduce)');
  const cards = [...root.querySelectorAll('[data-home-reveal]')];
  let observer;
  function motion() {
    observer?.disconnect();
    if (reduced.matches || !('IntersectionObserver' in window)) {
      cards.forEach(el => {el.classList.remove('hi-pending');el.classList.add('hi-visible');});
      return;
    }
    observer = new IntersectionObserver(entries => {
      // Animate only once; do not hide content again when scrolling back up.
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('hi-visible');
          observer.unobserve(entry.target);
        }
      });
    }, {threshold: 0.08, rootMargin: '0px 0px -35px 0px'});
    cards.forEach((el) => {
      if (el.classList.contains('hi-visible')) return;
      el.classList.add('hi-pending');
      const siblings = [...el.parentElement.children].filter(n => n.matches('[data-home-reveal]'));
      el.style.setProperty('--hi-delay', Math.min(siblings.indexOf(el), 3) * 140 + 'ms');
      observer.observe(el);
    });
  }
  motion();
  reduced.addEventListener?.('change', motion);
  // Keyboard users never have to wait for an animation to use a link.
  root.addEventListener('focusin', event => event.target.closest('[data-home-reveal]')?.classList.add('hi-visible'));
  const progress = root.querySelector('.hi-progress span');
  let scheduled = false;
  function update() {
    const max = document.documentElement.scrollHeight - innerHeight;
    progress.style.transform = `scaleX(${max > 0 ? Math.min(1, Math.max(0, scrollY / max)) : 0})`;
    document.body.classList.toggle('hi-scrolled', scrollY > 40);
    scheduled = false;
  }
  addEventListener('scroll', () => {if (!scheduled) {scheduled = true;requestAnimationFrame(update);}}, {passive:true});
  addEventListener('resize', update);
  update();
  // Fallback for missing catalog images without replacing correct product photos.
  root.querySelectorAll('.hi-product-image img').forEach(img => {
    const fallback = () => {img.hidden = true;img.parentElement.classList.add('hi-image-missing');};
    img.addEventListener('error', fallback, {once:true});
    if (img.complete && img.naturalWidth === 0) fallback();
  });
})();
