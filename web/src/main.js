import Vue from 'vue'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import App from './App.vue'
import router from './router'
import './style/iconfont.css'
import JsonViewer from 'vue-json-viewer'
import axios from 'axios'
import VueContextMenu from '@xunlei/vue-context-menu'
import GlobalContext from './Framework/GlobalContext'
import ContextMenuManager from './Framework/ContextMenuManager'
import './views/component/common'
import './views/component/loading/dialog.js'
import './views/component/ContextMenu.js'
import './views/dialog/dialog.js'
// 类型扩展方法
import store from './store'
import messageBox from './views/component/messageBox/index.js'
import './assets/font_class/iconfont.css'

// Import echarts
import * as echarts from 'echarts'
Vue.prototype.$echarts = echarts

// Import JsonViewer as a Vue.js plugin
Vue.use(JsonViewer)
Vue.use(ElementUI)
Vue.use(VueContextMenu)
Vue.use(messageBox.register)
window.$message =  Vue.prototype.$message

Vue.config.productionTip = false
axios.defaults.baseURL = process.env.VUE_APP_BASE_API
axios.defaults.withCredentials = true
axios.defaults.crossDomain = true
axios.defaults.headers.common['Access-Control-Allow-Origin'] = '*'

Vue.prototype.$http = axios
Vue.prototype.$ContextMenuManager = ContextMenuManager
Vue.prototype.$ContextMenuManager.InitContextMenuItems()
Vue.prototype.$store = store
window.$store = store

Vue.prototype.$GlobalContext = GlobalContext
window.$GlobalContext = GlobalContext
 
// 添加全局组件
import BaseTableSelectionMultiple from './views/component/common/BaseTableSelectionMultiple.vue';
import BaseCheckBoxGroup from './views/component/common/BaseCheckBoxGroup.vue';
import TooltipButton from './views/component/common/TooltipButton.vue';
import TooltipIcon from './views/component/common/TooltipIcon.vue';
import TooltipLink from './views/component/common/TooltipLink.vue';
Vue.component('BaseTableSelectionMultiple', BaseTableSelectionMultiple);
Vue.component('BaseCheckBoxGroup', BaseCheckBoxGroup);
Vue.component('TooltipButton', TooltipButton);
Vue.component('TooltipIcon', TooltipIcon);
Vue.component('TooltipLink', TooltipLink);

import { loadScripts } from './utils/dgraph'
loadScripts(() => {
  new Vue({
    render: (h) => h(App),
    router,
    store,
  }).$mount('#app')
})
