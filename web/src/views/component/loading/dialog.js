// 引入vue
import Vue from 'vue'
// 引入loading组件
import dialog from './index';

//创建Dialog构造器
let PopupBox = Vue.extend(dialog)
let instance
let index = 0
const show = function(data = {}) {
  Vue.nextTick(() => {
    if (instance) {
      // 已有弹窗，更新data
      Object.keys(data).forEach(key=>{
        instance[key] = data[key]
      })
      instance.showLoading = true
      return
    }
    // 挂载至页面
    instance = new PopupBox({
      data
    }).$mount()
    document.querySelector('#app').appendChild(instance.$el)
    instance.showLoading = true
    // 超时后允许手动关闭loading
    index ++ 
    let _index = index
    setTimeout(()=>{
      if(instance && _index == index){
        instance.cancancel = true
      }
    }, instance.timeout * 1000 )
  })
}

// 关闭弹框函数
function hide() {
  // 移除已有弹窗，确保只有一个弹窗显示
  if (instance) {
    instance.showLoading = false
    document.querySelector('#app').removeChild(instance.$el)
    instance = null
  }
}

// 将打开函数和关闭函数都挂载到Vue原型上，方便我们调用
Vue.prototype.$showLoading = show;
Vue.prototype.$hideLoading = hide;
window.$showLoading = show;
window.$hideLoading = hide;