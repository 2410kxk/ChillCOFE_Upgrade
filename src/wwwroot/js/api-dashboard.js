(() => {
    const el = id => document.getElementById(id);
    const fmt = n => Number(n).toLocaleString('vi-VN');
    const dateText = d => `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
    const now = new Date();el('to').value=dateText(now);el('from').value=dateText(new Date(now.getFullYear(),now.getMonth(),1));
    function clear() { ['revenue','paid','total','cancelled'].forEach(id=>el(id).textContent='—');el('rows').replaceChildren();el('bars').replaceChildren();el('empty').hidden=true;['daily-chart','orders-chart','daily-data','orders-legend'].forEach(id=>el(id).replaceChildren()); }
    async function read(url) {
        const r=await fetch(url,{credentials:'same-origin',cache:'no-store'});
        if(!r.ok || r.redirected) throw new Error(r.status===401||r.status===403||r.redirected ? 'Cần đăng nhập bằng tài khoản Admin.' : 'Không tải được dữ liệu. Kiểm tra khoảng ngày và kết nối.');
        return r.json();
    }
    function svg(tag,attrs,text) { const node=document.createElementNS('http://www.w3.org/2000/svg',tag);Object.entries(attrs).forEach(([k,v])=>node.setAttribute(k,v));if(text!==undefined)node.textContent=text;return node; }
    function charts(points,orders,start,end) {
        const data=new Map(points.map(p=>[p.date.slice(0,10),p.revenue]));const days=[];
        const cursor=new Date(start+'T12:00:00');const last=new Date(end+'T12:00:00');
        while(cursor<=last){const date=dateText(cursor);days.push({date,value:Number(data.get(date)||0)});cursor.setDate(cursor.getDate()+1);}
        const chart=el('daily-chart'),max=Math.max(1,...days.map(p=>p.value));
        for(let i=0;i<=4;i++){const y=190-i*40;chart.append(svg('line',{x1:80,y1:y,x2:580,y2:y,stroke:'#ded4c7'}),svg('text',{x:74,y:y+4,'text-anchor':'end','font-size':11,fill:'#594439'},fmt(max*i/4)));}
        const coords=days.map((p,i)=>[80+(days.length===1?250:i*500/(days.length-1)),190-p.value/max*160]);
        chart.append(svg('polyline',{points:coords.map(p=>p.join(',')).join(' '),fill:'none',stroke:'#774b2e','stroke-width':3}));
        days.forEach((p,i)=>{const dot=svg('circle',{cx:coords[i][0],cy:coords[i][1],r:days.length>60?2:4,fill:'#774b2e'});dot.append(svg('title',{},p.date+': '+fmt(p.value)+' đ'));chart.append(dot);const row=document.createElement('p');row.textContent=p.date+' — '+fmt(p.value)+' đ';el('daily-data').append(row);});
        chart.append(svg('text',{x:80,y:220,'font-size':12},start),svg('text',{x:580,y:220,'text-anchor':'end','font-size':12},end));
        const groups=[['Đã thanh toán',orders.paid,'#6b482d'],['Đang xử lý',orders.open,'#c29462'],['Đã hủy',orders.cancelled,'#ba695b'],['Khác',Math.max(0,orders.total-orders.paid-orders.open-orders.cancelled),'#b4b7ad']];
        const pie=el('orders-chart'),circ=2*Math.PI*75;let offset=0;
        pie.append(svg('circle',{cx:160,cy:110,r:75,fill:'none',stroke:'#e7dfd4','stroke-width':28}));
        groups.forEach(([name,count,color])=>{if(orders.total && count){const length=count/orders.total*circ;pie.append(svg('circle',{cx:160,cy:110,r:75,fill:'none',stroke:color,'stroke-width':28,'stroke-dasharray':length+' '+(circ-length),'stroke-dashoffset':-offset,transform:'rotate(-90 160 110)'}));offset+=length;}const item=document.createElement('span');item.textContent=name+': '+fmt(count);item.style.borderLeft='8px solid '+color;item.style.padding='0 12px';el('orders-legend').append(item);});
        pie.append(svg('text',{x:160,y:108,'text-anchor':'middle','font-size':28,fill:'#4b3124'},fmt(orders.total)),svg('text',{x:160,y:133,'text-anchor':'middle','font-size':13},orders.total?'Tổng số đơn':'Chưa có đơn'));
    }
    el('filters').addEventListener('submit',async e=>{
        e.preventDefault();clear();el('load').disabled=true;el('state').textContent='Đang tải…';
        try {
            if(!el('from').value || !el('to').value || el('from').value>el('to').value) throw new Error('Chọn ngày bắt đầu không sau ngày kết thúc.');
            const end=new Date(el('to').value+'T00:00:00');end.setDate(end.getDate()+1);
            const q=new URLSearchParams({from:el('from').value+'T00:00:00',to:dateText(end)+'T00:00:00'});
            const [r,o,b,d]=await Promise.all([read('/api/statistics/revenue?'+q),read('/api/statistics/orders?'+q),read('/api/statistics/best-sellers?'+q),read('/api/statistics/daily?'+q)]);
            charts(d.points,o,el('from').value,el('to').value);
            el('revenue').textContent=fmt(r.revenue)+' đ';el('paid').textContent=fmt(o.paid);el('total').textContent=fmt(o.total);el('cancelled').textContent=fmt(o.cancelled);
            const max=Math.max(1,...b.items.map(x=>x.quantity));
            for(const item of b.items) {
                const tr=document.createElement('tr');[item.name,fmt(item.quantity),fmt(item.grossAmount)+' đ'].forEach(text=>{const td=document.createElement('td');td.textContent=text;tr.append(td);});el('rows').append(tr);
                const bar=document.createElement('div');bar.textContent=item.name+' · '+fmt(item.quantity);const fill=document.createElement('span');fill.style.width=(item.quantity/max*100)+'%';bar.append(fill);el('bars').append(bar);
            }
            el('empty').hidden=b.items.length!==0;el('state').textContent='Đã cập nhật '+new Date().toLocaleTimeString('vi-VN');
        } catch(error) { clear();el('state').textContent=error.message; }
        finally {el('load').disabled=false;}
    });
    el('filters').requestSubmit();
})();
