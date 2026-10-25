const {request,chromium}=require(process.env.CODEX_PRIMARY_RUNTIME_NODE_MODULES+'/playwright');
const fs=require('fs');const results=[];
const check=(name,ok)=>{results.push({name,passed:!!ok});console.log((ok?'PASS ':'FAIL ')+name);};
const token=s=>s.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/)?.[1];
const decode=s=>s.replace(/&#x([0-9a-f]+);/gi,(_,x)=>String.fromCodePoint(parseInt(x,16)));
(async()=>{const contexts=[];let browser;
const {spawn}=require('child_process');const log=fs.openSync('/tmp/chill-online-host.log','w');
const host=spawn('/tmp/chill-dotnet/dotnet',['work/ChillCOFE_Upgrade/docs/verification-source/bin/Debug/net8.0/TestHost.dll'],{env:{...process.env,DOTNET_ROOT:'/tmp/chill-dotnet',ASPNETCORE_ENVIRONMENT:'Development',CHILL_TEST_DB:'/tmp/chill-online-verification.db'},stdio:['ignore',log,log]});

async function login(name){const c=await request.newContext({baseURL:'http://127.0.0.1:5088',maxRedirects:0});contexts.push(c);let html=await(await c.get('/Identity/Account/Login')).text();const r=await c.post('/Identity/Account/Login',{form:{'Input.Email':name+'@test.invalid','Input.Password':'TestOnly!234',__RequestVerificationToken:token(html)}});check('login '+name,r.status()===302);return c;}
async function post(c,path,data){let t=token(await(await c.get('/Online')).text());return c.post(path,{form:{...data,__RequestVerificationToken:t}});}
async function cart(c){return(await c.get('/Online/Cart')).json();}
try{
for(let i=0;i<80;i++){try{if((await fetch('http://127.0.0.1:5088/')).ok)break;}catch{}await new Promise(r=>setTimeout(r,250));}
const a=await login('alice'),b=await login('bob'),s=await login('staff'),admin=await login('admin');
for(const path of ['/','/Cafe/Menu','/Online','/Online/History'])check('render '+path,(await a.get(path)).status()===200);
check('render delivery staff',(await s.get('/Online/Deliveries')).status()===200);
check('render floor',(await s.get('/Pos/Tables')).status()===200);
check('user blocked deliveries',(await a.get('/Online/Deliveries')).status()===302);
check('CSRF blocked',(await a.post('/Online/Add',{form:{productId:1}})).status()===400);
check('add product',(await post(a,'/Online/Add',{productId:1})).status()===200);
check('cart isolated',(await cart(b)).count===0);
let c=await cart(a);const key=c.items[0].key;
check('invalid options blocked',(await post(a,'/Online/Update',{key,quantity:2,size:'XL',sugar:500,ice:100,topping:'Không'})).status()===400);
check('update options',(await post(a,'/Online/Update',{key,quantity:2,size:'L',sugar:25,ice:125,topping:'Kem cheese',note:'Ít ngọt nhé'})).status()===200);
c=await cart(a);check('server prices 2 x (35000+7000+10000)',c.total===104000&&c.items[0].ice===125);
check('invalid phone blocked',(await post(a,'/Online/Checkout',{name:'Alice',phone:'abc',address:'12 Test Street',paymentMethod:'Tiền mặt',checkoutKey:c.checkoutKey,expectedTotal:c.total})).status()===400);
check('price tamper blocked',(await post(a,'/Online/Checkout',{name:'Alice',phone:'0901234567',address:'12 Test Street',paymentMethod:'Tiền mặt',checkoutKey:c.checkoutKey,expectedTotal:1})).status()===409);
check('mark unavailable',(await post(admin,'/CatalogAdmin/Availability',{id:1,available:'false'})).status()===302);
check('unavailable add blocked',(await post(b,'/Online/Add',{productId:1})).status()===400);
const data={name:'Alice',phone:'0901234567',address:'12 Test Street',paymentMethod:'Chuyển khoản',checkoutKey:c.checkoutKey,expectedTotal:c.total};
check('unavailable checkout blocked',(await post(a,'/Online/Checkout',data)).status()===409);
check('reopen product',(await post(admin,'/CatalogAdmin/Availability',{id:1,available:'true'})).status()===302);
check('reopened add works',(await post(b,'/Online/Add',{productId:1})).status()===200);
let res=await post(a,'/Online/Checkout',data);check('checkout accepted',res.status()===200);const orderId=(await res.json()).orderId;
res=await post(a,'/Online/Checkout',data);check('retry same order',(await res.json()).orderId===orderId);
check('cart cleared',(await cart(a)).count===0);
check('history ownership',!decode(await(await b.get('/Online/History')).text()).includes('12 Test Street'));
let html=decode(await(await a.get('/Online/History')).text());check('history options saved',html.includes('125%')&&html.includes('Kem cheese')&&html.includes('Chờ xác nhận'));
check('user cannot complete',(await post(a,'/Online/Transition',{id:orderId,actionName:'complete',paymentConfirmed:true})).status()===302);
await post(s,'/Online/Transition',{id:orderId,actionName:'complete',paymentConfirmed:true});html=decode(await(await a.get('/Online/History')).text());check('cannot complete before shipping',html.includes('Chờ xác nhận'));
await post(s,'/Online/Transition',{id:orderId,actionName:'ship'});html=decode(await(await a.get('/Online/History')).text());check('shipping transition',html.includes('Đang giao'));
await post(s,'/Online/Transition',{id:orderId,actionName:'complete'});html=decode(await(await a.get('/Online/History')).text());check('payment confirmation required',html.includes('Đang giao'));
await post(s,'/Online/Transition',{id:orderId,actionName:'complete',paymentConfirmed:true});html=decode(await(await a.get('/Online/History')).text());check('delivery paid',html.includes('Giao hàng thành công')&&html.includes('Đã thanh toán'));
await post(s,'/Online/Transition',{id:orderId,actionName:'complete',paymentConfirmed:true});await post(s,'/Online/Transition',{id:orderId,actionName:'cancel'});html=decode(await(await a.get('/Online/History')).text());check('closed order cannot cancel',html.includes('Giao hàng thành công'));
check('staff table note',(await post(s,'/Online/TableNote',{id:1,note:'Gần cửa sổ — 4 khách'})).status()===302);check('note persisted',decode(await(await s.get('/Pos/Tables')).text()).includes('Gần cửa sổ'));
check('user blocked note',(await post(a,'/Online/TableNote',{id:1,note:'HACK'})).status()===302);
const bc=await cart(b);check('remove line',(await post(b,'/Online/Remove',{key:bc.items[0].key})).status()===200&&(await cart(b)).count===0);
check('order management render',(await admin.get('/Order/Manage')).status()===200);
const reg=await request.newContext({baseURL:'http://127.0.0.1:5088',maxRedirects:0});contexts.push(reg);
const rh=await(await reg.get('/Identity/Account/Register')).text();const rr=await reg.post('/Identity/Account/Register',{form:{'Input.Email':'newuser@test.invalid','Input.Password':'TestOnly!234','Input.ConfirmPassword':'TestOnly!234',Role:'Admin',__RequestVerificationToken:token(rh)}});check('new account registration',rr.status()===302);
// UI tests use local application and synthetic accounts only.
if(process.env.CHILL_BROWSER_PATH && fs.existsSync(process.env.CHILL_BROWSER_PATH)){
browser=await chromium.launch({executablePath:process.env.CHILL_BROWSER_PATH,headless:true,args:['--no-sandbox']});const context=await browser.newContext({storageState:await a.storageState(),viewport:{width:1440,height:1000}});const page=await context.newPage();const errors=[];page.on('pageerror',e=>errors.push(e.message));await page.goto('http://127.0.0.1:5088/Cafe/Menu');await page.locator('[data-online-add]').first().click();await page.waitForFunction(()=>document.querySelector('[data-online-count]').textContent==='1');check('UI toast and badge',await page.locator('#online-toast').isVisible());await page.goto('http://127.0.0.1:5088/Online');await page.locator('.online-line').waitFor();check('UI cart renders',await page.locator('.online-line').count()===1);await page.screenshot({path:'/tmp/chill-cart-desktop.png',fullPage:true});await page.goto('http://127.0.0.1:5088/');await page.screenshot({path:'/tmp/chill-home-desktop.png'});await page.setViewportSize({width:390,height:844});await page.reload();check('mobile no horizontal overflow',await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth));await page.screenshot({path:'/tmp/chill-home-mobile.png'});check('no JavaScript errors',errors.length===0);await context.close();
}else{results.push({name:"Browser visual/UI verification",passed:null,status:"blocked",detail:"Chromium unavailable; download endpoint returned Site Unavailable. HTML routes render and JS syntax checked separately."});console.log("BLOCKED Browser visual/UI verification");}
}catch(e){check('runner '+e.stack,false);}finally{host.kill();if(browser)await browser.close();for(const c of contexts)await c.dispose();fs.writeFileSync('work/ChillCOFE_Upgrade/docs/online-test-results.json',JSON.stringify({environment:'Isolated SQLite test database; not SQL Server',results},null,2));if(results.some(x=>x.passed===false))process.exitCode=1;}})();
