(() => {
  'use strict';
  const money = value => new Intl.NumberFormat('vi-VN').format(value || 0) + ' đ';
  const normalize = s => (s || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g,'d').toLowerCase();
  document.querySelectorAll('[data-catalog]').forEach(root => {
    let category = 'all';
    const cards = [...root.querySelectorAll('.ch-product')];
    const search = root.querySelector('[data-search]'), sort = root.querySelector('[data-sort]');
    function filter() {
      const term = normalize(search?.value);
      let count = 0;
      cards.forEach(card => { card.hidden = !(category === 'all' || card.dataset.category === category) || !normalize(card.dataset.name).includes(term); if (!card.hidden) count++; });
      root.querySelector('[data-count]').textContent = count + ' món';
      root.querySelector('[data-empty]').hidden = count > 0;
      const ordered = [...cards];
      if (sort?.value === 'asc') ordered.sort((a,b) => +a.dataset.price - +b.dataset.price);
      if (sort?.value === 'desc') ordered.sort((a,b) => +b.dataset.price - +a.dataset.price);
      ordered.forEach(card => root.querySelector('[data-products]').appendChild(card));
    }
    root.querySelectorAll('[data-filter]').forEach(button => button.addEventListener('click', () => {
      category = button.dataset.filter;
      root.querySelectorAll('[data-filter]').forEach(b => b.classList.toggle('active', b === button)); filter();
    }));
    search?.addEventListener('input', filter); sort?.addEventListener('change', filter); filter();
  });
  const root = document.querySelector('[data-pos]');
  if (!root) return;
  const $ = id => document.getElementById(id);
  const products = JSON.parse($('pos-products').textContent), tableId = +root.dataset.table;
  let cart = { orderId:0, total:0, items:[] }, chosen = null, busy = false, paySnapshot = null;
  let toastTimer;
  function toast(message) { $('ch-toast').textContent=message; $('ch-toast').hidden=false; clearTimeout(toastTimer); toastTimer=setTimeout(()=>{$('ch-toast').hidden=true;},3500); }
  async function post(action, data) {
    const response = await fetch('/Pos/'+action, { method:'POST', headers:{'Content-Type':'application/json','RequestVerificationToken':document.querySelector('input[name="__RequestVerificationToken"]').value}, body:JSON.stringify(data) });
    if (response.redirected) throw new Error('Phiên đăng nhập đã hết. Hãy đăng nhập lại.');
    const body = await response.json().catch(()=>({}));
    if (!response.ok) throw new Error(body.message || 'Yêu cầu chưa hoàn tất. Tải lại đơn để kiểm tra trước khi thử lại.');
    return body;
  }
  function element(tag, cls, text) { const node=document.createElement(tag); if(cls)node.className=cls; if(text!==undefined)node.textContent=text; return node; }
  async function refresh() {
    const response=await fetch('/Pos/Cart?tableId='+tableId,{cache:'no-store'});
    if(!response.ok||response.redirected) throw new Error('Không tải được đơn. Kiểm tra kết nối hoặc đăng nhập lại.');
    cart=await response.json(); cart.items=cart.items||[]; render();
  }
  function render() {
    const lines=$('cart-lines'); lines.replaceChildren();
    if(!cart.items.length)lines.append(element('p','ch-empty','Chưa có món nào. Chọn một món bên trái để bắt đầu.'));
    cart.items.forEach(item=>{
      const line=element('div','ch-cart-line'); line.append(element('h3','',item.name));
      line.append(element('p','',`Size ${item.size} · ${item.sugar}% đường · ${item.ice}% đá · ${item.topping}`));
      if(item.note)line.append(element('p','',item.note));
      const bottom=element('div','ch-line-bottom'), step=element('div','ch-stepper');
      for(const [label,qty] of [['−',item.quantity-1],['+',item.quantity+1]]) {
        const button=element('button','',label);button.type='button';button.setAttribute('aria-label',(label==='+'?'Tăng ':'Giảm ')+item.name);button.disabled=busy||qty>99;
        button.addEventListener('click',()=>change(item.id,qty));step.append(button);
        if(label==='−')step.append(element('span','',item.quantity));
      }
      bottom.append(step,element('strong','',money(item.quantity*item.price)));
      const remove=element('button','ch-remove','Xóa');remove.type='button';remove.disabled=busy;remove.addEventListener('click',()=>change(item.id,0));bottom.append(remove);
      line.append(bottom);lines.append(line);
    });
    $('cart-total').textContent=money(cart.total);$('cart-count').textContent=cart.items.reduce((n,x)=>n+x.quantity,0)+' món';$('checkout-open').disabled=busy||!cart.items.length;
  }
  async function change(detailId, quantity) {
    if(busy)return;busy=true;render();
    try {await post('Quantity',{tableId,orderId:cart.orderId,detailId,quantity});await refresh();}
    catch(e){toast(e.message);await refresh().catch(()=>{});}
    finally{busy=false;render();}
  }
  document.querySelectorAll('[data-close]').forEach(b=>b.addEventListener('click',()=>b.closest('dialog').close()));
  root.querySelectorAll('[data-pick]').forEach(button=>button.addEventListener('click',()=>{
    chosen=products.find(p=>p.id===+button.dataset.pick); if(!chosen)return;
    $('pick-form').reset();$('pick-name').textContent=chosen.name;$('pick-description').textContent=chosen.description||'';
    $('custom-fields').hidden=!chosen.custom;$('pick-error').textContent='';pickPrice();$('pick-dialog').showModal();
  }));
  function pickPrice(){if(!chosen)return;const topping={'Không':0,'Trân châu':5000,'Thạch trái cây':5000,'Kem cheese':10000};
    const amount=chosen.price+(chosen.custom&&$('pick-size').value==='L'?7000:0)+(chosen.custom?topping[$('pick-topping').value]:0);
    $('pick-price').textContent=money(amount*(+$('pick-qty').value||1));}
  ['pick-size','pick-topping','pick-qty'].forEach(id=>$(id).addEventListener('input',pickPrice));
  $('pick-form').addEventListener('submit',async e=>{e.preventDefault();if(busy||!chosen)return;busy=true;$('pick-submit').disabled=true;
    try{await post('Add',{tableId,productId:chosen.id,quantity:+$('pick-qty').value,size:$('pick-size').value,sugar:+$('pick-sugar').value,ice:+$('pick-ice').value,topping:$('pick-topping').value,note:$('pick-note').value});$('pick-dialog').close();await refresh();toast('Đã thêm món vào đơn.');}
    catch(err){$('pick-error').textContent=err.message;}
    finally{busy=false;$('pick-submit').disabled=false;render();}
  });
  $('checkout-open').addEventListener('click',async()=>{
    if(busy)return;busy=true;$('checkout-open').disabled=true;
    try{await refresh();if(!cart.items.length)return;paySnapshot={orderId:cart.orderId,total:cart.total};$('pay-form').reset();$('pay-total').textContent=money(cart.total);$('cash-input').value=cart.total;$('pay-error').textContent='';cashChange();$('pay-dialog').showModal();}
    catch(e){toast(e.message);}finally{busy=false;render();}
  });
  function cashChange(){const cash=$('pay-method').value==='Tiền mặt';$('cash-fields').hidden=!cash;$('transfer-note').hidden=cash;
    const change=+$('cash-input').value-(paySnapshot?.total||0);$('cash-change').textContent=change<0?'Còn thiếu '+money(-change):money(change);}
  $('cash-input').addEventListener('input',cashChange);$('pay-method').addEventListener('change',cashChange);
  $('pay-form').addEventListener('submit',async e=>{e.preventDefault();if(busy||!paySnapshot)return;busy=true;$('pay-submit').disabled=true;
    try{const result=await post('Pay',{tableId,orderId:paySnapshot.orderId,expectedTotal:paySnapshot.total,method:$('pay-method').value,customerPaid:+$('cash-input').value||0,confirmed:$('paid-confirm').checked});location.assign(result.receiptUrl);}
    catch(err){$('pay-error').textContent=err.message;await refresh().catch(()=>{});}
    finally{busy=false;$('pay-submit').disabled=false;render();}
  });
  refresh().catch(e=>{$('cart-lines').textContent=e.message;toast(e.message);});
})();
