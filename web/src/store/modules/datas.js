/**业务功能缓存数据，页面刷新后清除 */
export default {
  namespaced: false,
  state: {
    /**交互构建，设置发送方 */
    currentPublisher: null,
    // 交互时序组件的数据源
    DataFlowSequenceData: {},
    DataFlowSequenceSetting: {},
    // 故障树视图图元选中项
    SafetyAnalysisAllSelectedDiagramObjects: [],
    //故障数仿真
    FaulttreeSimulationid: [],
    //完整故障树数据
    FullFaultTreeSata: [],
    //策略诊断数据
    Policydiagnosticdata: [],
    graphPublics: [],
    SimulationID: -1,
    MTBFPredictionCheckResult: '',
    // 历史辅以数据
    dataBaseFormData: {},
    closeMore: false,

    //策略诊断故障树数据
    PublicFaultTree:[],
  },
  mutations: {
    currentPublisher(state, val) {
      state.currentPublisher = val
    },
    DataFlowSequenceData(state, val) {
      state.DataFlowSequenceData = val
    },
    DataFlowSequenceSetting(state, val) {
      state.DataFlowSequenceSetting = val
    },
    dataBaseFormData(state, val) {
      state.dataBaseFormData = val
    },
    closeMore(state, val) {
      state.closeMore = val
    },
    SafetyAnalysisAllSelectedDiagramObjects(state, val) {
      state.SafetyAnalysisAllSelectedDiagramObjects = val
    },
    SimulationID(state, val) {
      state.SimulationID = val
    },
    FaulttreeSimulationid(state, val) {
      state.FaulttreeSimulationid = val
    },
    Policydiagnosticdata(state, val) {
      state.Policydiagnosticdata = val
    },
    graphPublics(state, val) {
      state.graphPublics = val
    },
    PublicFaultTree(state, val) {
      state.PublicFaultTree = val
    },
    FullFaultTreeSata(state, val) {
      state.FullFaultTreeSata = val
    },
    MTBFPredictionCheckResult(state, val) {
      state.MTBFPredictionCheckResult = val
    },
  },
  getters: {
    currentPublisher: function (state) {
      return state.currentPublisher
    },
    DataFlowSequenceData: function (state) {
      return state.DataFlowSequenceData
    },
    DataFlowSequenceSetting: function (state) {
      return state.DataFlowSequenceSetting
    },
    dataBaseFormData: function (state) {
      return state.dataBaseFormData
    },
    closeMore: function (state) {
      return state.closeMore
    },
    SafetyAnalysisAllSelectedDiagramObjects: function (state) {
      return state.SafetyAnalysisAllSelectedDiagramObjects
    },
    SimulationID: function (state) {
      return state.SimulationID
    },
    FaulttreeSimulationid: function (state) {
      return state.FaulttreeSimulationid
    },
    Policydiagnosticdata: function (state) {
      return state.Policydiagnosticdata
    },
    graphPublics: function (state) {
      return state.graphPublics
    },
    PublicFaultTree: function (state) {
      return state.PublicFaultTree
    },
    FullFaultTreeSata: function (state) {
      return state.FullFaultTreeSata
    },
    MTBFPredictionCheckResult: function (state) {
      return state.MTBFPredictionCheckResult
    },
  },
}
