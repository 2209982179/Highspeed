/**布局配置，持久化存储 */
export default {
  namespaced: true,
  state: {
    showProjectTree: true,
    showStartTree: true,
    showToolbox: false,
    showPropertyEditor: false,
    showPublisherInfo: true,
    showSafetyAnalysisDevice: false,
    showMTBDiagnosticStrategies: false,
    showSafetyAnalysisDeviceSimulation: false,
    showTestSimulationFor03Project: false,
    showMTBPolicyDiagnosticsSingle: false,
    showSafetyAnalysisDeviceAndFCSimulationEquipment: false,
    showDataFlowSequenceRunView: true,
    showStyleEditer: false,
    showModelStructure: true,
    showModelInfo: true,
  },
  mutations: {
    showProjectTree(state, val) {
      state.showProjectTree = val
      localStorage.setItem('layout.showProjectTree', val)
    },
    showStartTree(state, val) {
      state.showStartTree = val
      localStorage.setItem('layout.showStartTree', val)
    },
    showToolbox(state, val) {
      state.showToolbox = val
      localStorage.setItem('layout.showToolbox', val)
    },
    showPropertyEditor(state, val) {
      state.showPropertyEditor = val
      localStorage.setItem('layout.showPropertyEditor', val)
    },
    showPublisherInfo(state, val) {
      state.showPublisherInfo = val
    },
    showSafetyAnalysisDevice(state, val) {
      state.showSafetyAnalysisDevice = val
    },
    showMTBDiagnosticStrategies(state, val) {
      state.showMTBDiagnosticStrategies = val
    },
    showSafetyAnalysisDeviceSimulation(state, val) {
      state.showSafetyAnalysisDeviceSimulation = val
    },
    showTestSimulationFor03Project(state, val) {
      state.showTestSimulationFor03Project = val
    },
    showMTBPolicyDiagnosticsSingle(state, val) {
      state.showMTBPolicyDiagnosticsSingle = val
    },
    showSafetyAnalysisDeviceAndFCSimulationEquipment(state, val) {
      state.showSafetyAnalysisDeviceAndFCSimulationEquipment = val
    },
    showDataFlowSequenceRunView(state, val) {
      state.showDataFlowSequenceRunView = val
    },
    showStyleEditer(state, val) {
      state.showStyleEditer = val
    },
    showModelStructure(state, val) {
      state.showModelStructure = val
      localStorage.setItem('layout.showModelStructure', val)
    },
    showModelInfo(state, val) {
      state.showModelInfo = val
      localStorage.setItem('layout.showModelInfo', val)
    },
  },
  getters: {
    showProjectTree: function (state) {
      return state.showProjectTree
    },
    showStartTree: function (state) {
      return state.showStartTree
    },
    showToolbox: function (state) {
      return state.showToolbox
    },
    showPropertyEditor: function (state) {
      return state.showPropertyEditor
    },
    showPublisherInfo: function (state) {
      return state.showPublisherInfo
    },
    showSafetyAnalysisDevice: function (state) {
      return state.showSafetyAnalysisDevice
    },
    showMTBDiagnosticStrategies: function (state) {
      return state.showMTBDiagnosticStrategies
    },
    showSafetyAnalysisDeviceSimulation: function (state) {
      return state.showSafetyAnalysisDeviceSimulation
    },
    showTestSimulationFor03Project: function (state) {
      return state.showTestSimulationFor03Project
    },
    showMTBPolicyDiagnosticsSingle: function (state) {
      return state.showMTBPolicyDiagnosticsSingle
    },
    showSafetyAnalysisDeviceAndFCSimulationEquipment: function (state) {
      return state.showSafetyAnalysisDeviceAndFCSimulationEquipment
    },
    showDataFlowSequenceRunView: function (state) {
      return state.showDataFlowSequenceRunView
    },
    showStyleEditer: function (state) {
      return state.showStyleEditer
    },
    showModelStructure: function (state) {
      return state.showModelStructure
    },
    showModelInfo: function (state) {
      return state.showModelInfo
    },
  },
}
