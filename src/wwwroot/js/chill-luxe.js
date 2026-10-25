(() => {
'use strict';
const motion=matchMedia('(prefers-reduced-motion: reduce)');
const toggle=document.querySelector('.lx-menu-toggle');
const nav=document.querySelector('.lx-nav');
const menus=[...document.querySelectorAll('.lx-admin')];
function closeMenu(){if(toggle)toggle.setAttribute('aria-expanded','false');if(nav)nav.classList.remove('is-open');}
if(toggle&&nav){toggle.addEventListener('click',()=>{const open=toggle.getAttribute('aria-expanded')!=='true';toggle.setAttribute('aria-expanded',String(open));nav.classList.toggle('is-open',open);});nav.querySelectorAll('a').forEach(a=>a.addEventListener('click',closeMenu));}
document.addEventListener('click',e=>menus.forEach(d=>{if(!d.contains(e.target))d.open=false;}));
document.addEventListener('keydown',e=>{if(e.key==='Escape'){menus.forEach(d=>{if(d.open){d.open=false;d.querySelector('summary')?.focus();}});if(toggle?.getAttribute('aria-expanded')==='true'){closeMenu();toggle.focus();}}});
document.querySelectorAll('.lx-nav>a').forEach(a=>{if(a.pathname===location.pathname)a.setAttribute('aria-current','page');});
const reveals=[...document.querySelectorAll('.lx-reveal,.lx-favorites .ch-product,.lx-ritual article')];
if(!motion.matches&&'IntersectionObserver' in window){const observer=new IntersectionObserver(entries=>entries.forEach(entry=>{if(entry.isIntersecting){entry.target.classList.add('is-visible');observer.unobserve(entry.target);}}),{threshold:.06,rootMargin:'0px 0px 25px 0px'});reveals.forEach((el,i)=>{el.classList.add('cf-reveal-pending');el.style.setProperty('--cf-delay',`${Math.min(i%4,3)*55}ms`);observer.observe(el);});}
const header=document.querySelector('.lx-header');const back=document.querySelector('.lx-back-top');
const images=[...document.querySelectorAll('.lx-hero-art>img,.lx-story-image>img')];let queued=false;
function paint(){queued=false;if(back)back.hidden=scrollY<600;header?.classList.toggle('is-scrolled',scrollY>24);images.forEach(img=>{const rect=img.parentElement.getBoundingClientRect();const offset=motion.matches?0:Math.max(-12,Math.min(12,(innerHeight/2-rect.top-rect.height/2)*.025));img.style.setProperty('--cf-shift',`${offset.toFixed(2)}px`);});}
function queue(){if(!queued){queued=true;requestAnimationFrame(paint);}}
addEventListener('scroll',queue,{passive:true});addEventListener('resize',queue);paint();
back?.addEventListener('click',()=>scrollTo({top:0,behavior:motion.matches?'auto':'smooth'}));
motion.addEventListener('change',()=>{reveals.forEach(el=>el.classList.add('is-visible'));queue();});
document.querySelectorAll('[data-reel]').forEach(reel=>{const button=reel.querySelector('.cf-reel-toggle');button?.addEventListener('click',()=>{const paused=reel.classList.toggle('is-paused');button.setAttribute('aria-pressed',String(paused));button.textContent=paused?'▶':'Ⅱ';button.setAttribute('aria-label',paused?'Tiếp tục chuyển ảnh':'Dừng chuyển ảnh');});});
})();
