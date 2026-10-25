(() => {
const wrap=document.querySelector('.mega-wrap'),trigger=document.querySelector('.mega-trigger');
if(wrap&&trigger){
 const setOpen=open=>{wrap.classList.toggle('is-open',open);trigger.setAttribute('aria-expanded',String(open));};
 trigger.addEventListener('click',()=>setOpen(!wrap.classList.contains('is-open')));
 wrap.addEventListener('pointerenter',e=>{if(e.pointerType==='mouse')setOpen(true);});
 wrap.addEventListener('pointerleave',e=>{if(e.pointerType==='mouse'&&!wrap.querySelector('.drink-mega').contains(document.activeElement))setOpen(false);});
 wrap.addEventListener('focusout',e=>{if(!wrap.contains(e.relatedTarget))setOpen(false);});
 document.addEventListener('keydown',e=>{if(e.key==='Escape'){setOpen(false);if(wrap.contains(document.activeElement))trigger.focus();}});
 document.addEventListener('click',e=>{if(!wrap.contains(e.target))setOpen(false);});
}
document.querySelectorAll('form[data-confirm]').forEach(f=>f.addEventListener('submit',e=>{if(!confirm(f.dataset.confirm))e.preventDefault();}));
document.querySelectorAll('.ux-worknav a').forEach(a=>{if(a.pathname===location.pathname)a.setAttribute('aria-current','page');});
document.querySelectorAll('input[type=file][name="Image"],input[type=file][name="Avatar"]').forEach(input=>input.addEventListener('change',()=>{if(input.files[0]?.size>5*1024*1024){input.setCustomValidity('Ảnh tối đa 5 MB.');input.reportValidity();}else input.setCustomValidity('');}));
})();