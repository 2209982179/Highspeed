// 导入 vue-router
import Vue from 'vue'
import VueRouter from 'vue-router'
//注册 vue-router
Vue.use(VueRouter)
const originalPush = VueRouter.prototype.push
VueRouter.prototype.push = function push(location) {
  return originalPush.call(this, location).catch((err) => err)
}
// 实例化
const router = new VueRouter({
  //这里就是路由的配制项
  routes: [
    {
      path: '/',
      name: 'login',
      component: () => import('../views/login/login.vue'),
    },
    {
      path: '/ribbonBarView',
      name: 'ribbonBarView',
      component: () => import('../views/ribbonBarView/ribbonBarView.vue'),
    },
    {
      path: '/TestCaseList',
      name: 'TestCaseList',
      component: () =>
        import('../views/business/TestCaseList/TestCaseList.vue'),
    },
    {
      path: '/TestCaseDraw',
      name: 'TestCaseDraw',
      component: () => import('../views/business/MXGraghTest/MXGraghTest.vue'),
    },
  ],
})

export default router
