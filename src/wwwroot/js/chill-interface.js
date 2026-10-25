/* View-only enhancements; no API endpoints or transaction rules changed. */
(() => {
'use strict';
const fold = value => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g,'d').replace(/Đ/g,'D').toLocaleLowerCase('vi').trim();
document.querySelectorAll('[data-row-search]').forEach(input => {
 const name=input.dataset.rowSearch; const target=document.getElementById(name); if(!target)return;
 const rows=[...target.querySelectorAll('[data-search-row]')];
 const count=document.querySelector('[data-search-count="'+name+'"]');
 const empty=document.querySelector('[data-search-empty="'+name+'"]');
 function filter(){const q=fold(input.value);let visible=0;rows.forEach(row=>{const show=fold(row.textContent).includes(q);row.hidden=!show;if(show)visible++;});if(count)count.textContent=visible+' / '+rows.length+' mục trong danh sách';if(empty)empty.hidden=visible>0 || !q;}
 input.addEventListener('input',filter);filter();
});
const route=location.pathname.replace(/\/$/,'').toLowerCase();
document.querySelectorAll('.ux-worknav nav a').forEach(a=>{const href=a.getAttribute('href').toLowerCase();const segment=href.split('/')[1];if(route===href || (route.startsWith('/'+segment+'/') && segment!=='pos') || (segment==='pos' && route.startsWith('/pos/')))a.setAttribute('aria-current','page');});
document.querySelectorAll('.ux-category-links a').forEach(a=>{const u=new URL(a.href,location.origin);if(u.pathname===location.pathname && u.search===location.search)a.setAttribute('aria-current','page');});
const file=document.querySelector('[data-image-upload]');const preview=document.querySelector('[data-upload-preview]');
if(file && preview){const initial=preview.src;let active='';const error=document.querySelector('[data-upload-error]');file.addEventListener('change',()=>{if(active){URL.revokeObjectURL(active);active='';}preview.src=initial;if(error)error.textContent='';const selected=file.files[0];if(!selected)return;if(selected.size>5*1024*1024 || !/\.(jpe?g|png|webp|avif)$/i.test(selected.name)){if(error)error.textContent='Chọn ảnh JPG, PNG, WebP hoặc AVIF không quá 5 MB.';file.value='';return;}active=URL.createObjectURL(selected);preview.src=active;});window.addEventListener('pagehide',()=>{if(active)URL.revokeObjectURL(active);});}
})();
