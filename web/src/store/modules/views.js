/**视图管理 */
export default {
  namespaced: false,
  state: {
    /**当前打开的视图 */
    currentView: {},
    currentViewData: {},
    /**当前打开的视图（底部视图） */
    currentShowViews: [],
    /**四性设计tabkey */
    currentTabKey: '',
    /**当前打开的dialog数量 */
    dialogs: 0,
    selectedNodesz: 0,
    transmitFailureId:[],
  },
  mutations: {
    currentView(state, val) {
      state.currentView = val
    },
    currentViewData(state, val) {
      state.currentViewData = val
    },
    selectedNodesz(state, val) {
      state.selectedNodesz = val
    },
    transmitFailureId(state, val) {
      state.transmitFailureId = val
    },
    currentShowViews(state, val) {
      state.currentShowViews = val
    },
    currentTabKey(state, val) {
      state.currentTabKey = val
    },
    dialogs(state, val) {
      state.dialogs = val
    },
  },
  getters: {
    currentView: function (state) {
      return state.currentView
    },
    currentViewData: function (state) {
      return state.currentViewData
    },
    selectedNodesz: function (state) {
      return state.selectedNodesz
    },
    transmitFailureId: function (state) {
      return state.transmitFailureId
    },
    currentShowViews: function (state) {
      return state.currentShowViews
    },
    currentTabKey: function (state) {
      return state.currentTabKey
    },
    dialogs: function (state) {
      return state.dialogs
    },
  },
}
