/**画布配置，持久化存储 */
export default {
  namespaced: false,
  state: {
    /**当前打开的图 */
    currentDiagram: {},
    /**当前打开的画布 */
    currentGraph: null,
    /**是否显示缩略图 */
    showMapView: null,
    /**是否显示图信息 */
    showDiagramDetailView: null,
    /**当前图中选中的元素 */
    currentSelectedDiagramObjects: [],
    /**当前图中选中的连线 */
    currentSelectedDiagramLinks: [],
    /**当前图中选中的图元（块或线） */
    currentSelectedDiagramCells: [],
  },
  mutations: {
    currentDiagram(state, val) {
      state.currentDiagram = val
    },
    currentGraph(state, val) {
      state.currentGraph = val
    },
    currentSelectedDiagramObjects(state, val) {
      state.currentSelectedDiagramObjects = val
    },
    showMapView(state, val) {
      state.showMapView = val
      localStorage.setItem('config.showMapView', val == true ? 'true' : 'false')
    },
    showDiagramDetailView(state, val) {
      state.showDiagramDetailView = val
      localStorage.setItem(
        'config.showDiagramDetailView',
        val == true ? 'true' : 'false'
      )
    },
  },
  getters: {
    currentDiagram: function (state) {
      return state.currentDiagram
    },
    currentSelectedDiagramObjects: function (state) {
      return state.currentSelectedDiagramObjects
    },
    currentGraph: function (state) {
      return state.currentGraph
    },
    showMapView: function (state) {
      // 首次取值，从缓存中获取
      if (state.showMapView == null) {
        state.showMapView = localStorage.getItem('config.showMapView') == 'true'
      }
      return state.showMapView
    },
    showDiagramDetailView: function (state) {
      // 首次取值，从缓存中获取
      if (state.showDiagramDetailView == null) {
        state.showDiagramDetailView =
          localStorage.getItem('config.showDiagramDetailView') == 'true'
      }
      return state.showDiagramDetailView
    },
    /**当前图中选中的连线 */
    currentSelectedDiagramLinks: function (state) {
      let cells = state.currentGraph?.getSelectionCells() ?? []
      if (cells.length == 0) {
        return []
      }
      let items = []
      cells.forEach((element) => {
        if (element.edge) {
          items.push(element)
        }
      })
      return items
    },
    /**当前图中选中的图元（块或线） */
    currentSelectedDiagramCells: function (state) {
      return state.currentGraph?.getSelectionCells() ?? []
    },
  },
}
