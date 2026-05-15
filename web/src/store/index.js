import Vue from 'vue'
import Vuex from 'vuex'
import config from './modules/config'
import layout from './modules/layout'
import user from './modules/user'
import graph from './modules/graph'
import views from './modules/views'
import datas from './modules/datas'
Vue.use(Vuex)
const store = new Vuex.Store({
  modules: {
    config,
    layout,
    user,
    graph,
    views,
    datas,
  },
  state: {
    /**当前访问的项目信息 */
    projectInfo: {},
    /**工程树选中项 */
    projectTreeSelectedItems: {},
    /**工具箱中选中的连线类型，用于创建连线 */
    currentLinkToolItem: {},
    reloadCurrentDiagramRandom: 0.1,
  },
  mutations: {
    projectInfo(state, val) {
      state.projectInfo = val
    },
    projectTreeSelectedItems(state, val) {
      state.projectTreeSelectedItems = val
    },
    currentLinkToolItem(state, val) {
      state.currentLinkToolItem = val
    },
    reloadCurrentDiagramRandom(state, val) {
      state.reloadCurrentDiagramRandom = val
    },
  },
  getters: {
    projectInfo: function (state) {
      return state.projectInfo
    },
    projectTreeSelectedItems: function (state) {
      return state.projectTreeSelectedItems
    },
    currentLinkToolItem: function (state) {
      return state.currentLinkToolItem
    },
    reloadCurrentDiagramRandom: function (state) {
      return state.reloadCurrentDiagramRandom
    },
  },
})
export default store
