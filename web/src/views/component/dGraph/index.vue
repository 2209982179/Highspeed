<template>
  <div class="box" ref="box">
    <!-- 画布 -->
    <div class="graphbox">
      <div
        @wheel="handleScroll"
        ref="container"
        :class="{ 'graphContainer-background': showBackground }"
        class="graphContainer"
        style="outline: 0"
      ></div>
    </div>
    <!--显示画布比例的box-->
    <div class="scale-box">比例:{{ dynamicScale }}%</div>
    <!-- 缩略图 -->
    <div class="show-map" ref="showMap" v-show="showMapView"></div>
    <!-- 图信息 -->
    <div
      class="show-diagram-detail"
      v-if="diagram"
      v-show="showDiagramDetailView"
    >
      <div>
        <span class="title">作者:</span>
        <span>{{ diagram.Author }}</span>
      </div>
      <div>
        <span class="title">名称:</span>
        <span>{{ diagram.RealName }}</span>
      </div>
      <div>
        <span class="title">类型:</span>
        <span>{{ diagram.Stereotype }}</span>
      </div>
      <div>
        <span class="title">所属:</span>
        <span>{{ diagram.Owner?.RealName }}</span>
      </div>
      <div>
        <span class="title">修改时间:</span>
        <span>{{ diagram.ModifiedDate }}</span>
      </div>
    </div>
  </div>
</template>
<script>
  import { createGraph } from '@/utils/dgraph'
  import {
    SaveToDiagram,
    ShowElementRefDiagram,
    GetEdgeStyleByStereotype,
    GetMxGraphXml,
    GetLinkType,
    LayoutDiagram,
    GetMxGraphImage,
    SaveToDiagramConnector,
    SetStereotypeVisible,
  } from '@/API/Adap'
  import { GetSubsystem } from '@/utils/subsystemUtils'
  import { v4 as uuid } from 'uuid'
  import mxgraph from '@/mxgraph/graph'
  import UploadDiagramXML from '@/views/dialog/UploadDiagramXML'
  import { portOrderAjustment } from '@/utils/portArrange'
  import { SeuenceToolSet } from '@/sysml/sequence'
  import { StateMachineToolSet } from '@/sysml/stateMachine'
  import { RestyleCell as RestyleHistory } from '@/utils/DiagramUtils/SysML/Cells/History'
  import { GetFlowSequenceSetting } from '@/views/dialog/DataFlowSequence/DataFlowSequenceSettingActions.js'

  // 必须显示的右键菜单key
  const MENU_MUST = [
    'ElementAddin.Locate', // 定位
    'ElementAddin.Contains', // 包含模型的图
  ]
  const {
    mxEvent,
    mxGraph,
    mxUtils,
    mxCodec,
    mxUndoManager,
    mxClient,
    mxPoint,
    mxGraphHandler,
    mxClipboard,
    ActiveXObject,
    mxPopupMenu,
  } = mxgraph

  export default {
    props: ['diagramId', 'diagram'],
    data() {
      return {
        graphApp: {},
        observer: {},
        editor: null,
        graph: null,
        graphX: 100, // 记录当前鼠标位置
        graphY: 100, // 记录当前鼠标位置
        mxUndoManager: '',
        showBackground: false,
        lastClickTarget: { id: 0 }, // 记录上一次点击的元素
        lastClickTime: 0, // 记录上一次点击的时间
        dynamicScale: 100, //记录实时比例
        tempGraphScale: 1, // 记录当前缩放比例，用于还原比例
        tempSelectedDiagramObjectIds: [], // 记录当前选中项，用于还原比例
        tempAddEdge: null, //通过拖曳创建的连线预览
        // 元素拖曳上下文数据
        contextJsonData: {
          cells: {
            nodes: [],
            groups: [],
          },
          edges: [],
        },
        autoSave: false,
        loading: false,
        currentSelectedDiagramObjectsCache: [],
      }
    },
    computed: {
      showMapView: {
        get() {
          return this.$store.getters['showMapView']
        },
        set(val) {
          this.$store.commit('showMapView', val)
        },
      },
      showDiagramDetailView: {
        get() {
          return this.$store.getters['showDiagramDetailView']
        },
        set(val) {
          this.$store.commit('showDiagramDetailView', val)
        },
      },
      currentSelectedDiagramLinks() {
        return this.$store.getters['currentSelectedDiagramLinks']
      },
      selectionCells() {
        return this.graph?.getSelectionCells()
      },
    },
    watch: {
      // 选中项变更时更新 currentSelectedDiagramObjects
      selectionCells(newVal, oldVal) {
        if (
          oldVal == null ||
          newVal.length != oldVal.length ||
          newVal.some((v, i) => v !== oldVal[i])
        ) {
          if (newVal.length == 0) {
            // 无选中项
            this.$store.commit('currentSelectedDiagramObjects', [])
            return
          }
          let items = []
          newVal.forEach((element) => {
            if (element.vertex) {
              items.push(element)
            }
          })
          this.$store.commit('currentSelectedDiagramObjects', items)
        }
      },
    },
    methods: {
      // 创建画布并进行初始化
      createGraph() {
        // 创建graph
        this.graphApp = createGraph(this.$refs.container)
        this.editor = this.graphApp.editor
        this.graph = this.graphApp.editor.graph
        this.$set(this.diagram, 'editor', this.editor)
        this.$set(this.diagram, 'graph', this.graph)
        this.$set(this.diagram, 'graphApp', this.graphApp)
        this.$set(this.diagram, 'diagramEditor', this)
        // 保存图
        this.$set(this.diagram, 'Save', this.SaveToDiagram)
        // 设置图不可撤回
        this.$set(this.diagram, 'setUndo', this.setUndo)
        // 刷新图，与后台保持一致，不检查当前是否已存在
        this.$set(this.diagram, 'Show', this.ShowDiagram)
        // 刷新图，与后台保持一致，检查当前是否已存在
        this.$set(this.diagram, 'Reload', this.ReloadDiagram)
        // 创建缩略图
        this.outline = this.graphApp.createOutline(this.$refs.showMap)
        // 自适应大小
        this.$set(this.diagram, 'autoSize', this.autoSize)

        // 图元的文本源
        this.graph.convertValueToString = (cell) => {
          return cell['value']

          // var hideLabel = cell['HideLabel']
          //     ? JSON.parse(cell['HideLabel'])
          //     : false
          // var hideStereotype = cell['HideStereotype']
          //     ? JSON.parse(cell['HideStereotype'])
          //     : false
          // if (hideLabel && hideStereotype) return ''

          // if (!hideLabel && cell['RealName']) return cell['RealName']
          // else if (!hideLabel && cell['value']) return cell['value']
          // else if (!hideStereotype && cell['title']) return cell['title']
          // else return ''
        }
        // this.graph.addListener(mxEvent.IS_POINTER, (sender, evt) => {
        //     // console.log('addListener  IS_POINTER',sender, evt);
        //     if (evt.name == 'zoomPreview') {
        //         console.log('addListener  IS_POINTER    zoomPreview',sender, evt);
        //     }
        // })
        // 禁止连接线晃动(即连线两端必须在节点上)
        // let SysMLDiagrams = ['Sequence']
        // this.graph.setAllowDanglingEdges(SysMLDiagrams.includes(this.diagram.Stereotype))
        this.graph.setAllowDanglingEdges(false)
        // this.diagramDetail = this.$store.getters['projectTreeSelectedItems'][0]
      },
      // 基础配置函数
      eventCenter() {
        let vm = this
        let graph = vm.diagram.graph
        let orderEdges = (graph) => {
          // 在画布中连线的图层要高于其他元素
          let edges = []
          for (var _key in graph.model.cells) {
            let _edge = graph.model.cells[_key]
            if (_edge.edge) {
              edges.push(_edge)
            }
          }
          graph.orderCells(false, edges)
        }

        // 注册自定义图元事件
        for (let key in mxEvent) {
          if (typeof mxEvent[key] == 'string') {
            graph.addListener(mxEvent[key], (sender, evt) => {
              vm.$nextTick(() => {
                let cells = evt.properties.cells
                if (cells?.length > 0 == false) {
                  return
                }
                window.$SmartCell.Actions.forEach((_action) => {
                  if (_action.condition(vm.diagram, cells) && _action[key]) {
                    try {
                      graph.model.beginUpdate()
                      _action[key](vm.diagram, cells, evt)
                    } finally {
                      graph.model.endUpdate()
                    }
                  }
                })
              })
            })
          }
        }

        // 新增图元、连线
        graph.addListener(mxEvent.ADD_CELLS, (sender, evt) => {
          this.$nextTick(async () => {
            let addCell = evt.properties.cells[0]
            if (addCell.vertex) {
              // 判断是否为组节点
              if (addCell.isGroup) {
                this.$message.info('添加了一个组')
                let groupObj = _.pick(addCell, [
                  'id',
                  'title',
                  'parent',
                  'geometry',
                ])
                this.contextJsonData['cells']['groups'].push(groupObj)
              } else {
                let nodeObj = _.pick(addCell, [
                  'id',
                  'title',
                  'parent',
                  'geometry',
                ])
                this.contextJsonData['cells']['nodes'].push(nodeObj)
                // this.$message.info('添加了一个节点')
              }
              // 向contextJsonData中更新数据

              // 加入SysML元素的处理
              if (
                addCell.Stereotype == 'StateNode_History' ||
                addCell.Stereotype == 'History'
              ) {
                await RestyleHistory(vm.diagram, addCell)
              } else if (addCell.Object_Type == 'InteractionFragment') {
                // InteractionFragment至少有一个区域
                await window.$DiagramUtils.SequenceStyle.AddFragmentItem(
                  vm.diagram,
                  addCell
                )
              }
            } else if (addCell.edge) {
              if (addCell.source?.ProhibitConnector == true) {
                vm.graph.removeCells([addCell])
                vm.tempAddEdge = null
                vm.$message.info('元素禁止添加连线')
                return
              } else if (addCell.target?.ProhibitConnector == true) {
                vm.graph.removeCells([addCell])
                vm.tempAddEdge = null
                vm.$message.info('目标禁止添加连线')
                return
              }
              if (!addCell.title) {
                this.tempAddEdge = addCell
                let toolItem = this.$store.getters['currentLinkToolItem']
                if (toolItem && toolItem.id) {
                  // 加入从工具箱中指定了类型的连线
                  this.createEdge(toolItem, addCell)
                  this.$store.commit('currentLinkToolItem', null)
                } else {
                  // 快速创建连线；加入未指定类型的连线，提供类型选择
                  let startStereoType = addCell.source.Stereotype ?? ''
                  let endStereoType = addCell.target.Stereotype ?? ''
                  if ((startStereoType && endStereoType) == false) {
                    return
                  }
                  let vm = this
                  let stypes = []
                  if (
                    startStereoType == 'Sequence' &&
                    endStereoType == 'Sequence'
                  ) {
                    // 加入时序图消息类型
                    let _stypes = SeuenceToolSet.filter((item) => {
                      if (item.Group.indexOf('Relationships') > -1) {
                        return true
                      }
                    })
                    stypes.push(..._stypes)
                  } else if (
                    addCell.source.Object_Type == 'StateNode' &&
                    addCell.target.Object_Type == 'State'
                  ) {
                    let _stypes = StateMachineToolSet.filter((item) => {
                      if (item.Group.indexOf('Relationships') > -1) {
                        return true
                      }
                    })
                    stypes.push(..._stypes)
                  } else if (
                    addCell.source.Object_Type == 'State' &&
                    addCell.target.Object_Type == 'StateNode'
                  ) {
                    let _stypes = StateMachineToolSet.filter((item) => {
                      if (item.Group.indexOf('Relationships') > -1) {
                        return true
                      }
                    })
                    stypes.push(..._stypes)
                  } else if (
                    addCell.source.Object_Type == 'State' &&
                    addCell.target.Object_Type == 'State'
                  ) {
                    let _stypes = StateMachineToolSet.filter((item) => {
                      if (item.Group.indexOf('Relationships') > -1) {
                        return true
                      }
                    })
                    stypes.push(..._stypes)
                  } else {
                    let _stypes = await GetLinkType({
                      startStereoType,
                      endStereoType,
                    })
                    if (_stypes) {
                      stypes.push(..._stypes)
                    } else {
                      vm.$message.error('获取关系类型失败')
                      vm.graph.removeCells([this.tempAddEdge])
                      vm.tempAddEdge = null
                      return
                    }
                  }

                  let menuItem = []
                  // 加入取消按钮
                  menuItem.push({
                    title: '取消',
                    action: () => {
                      vm.graph.removeCells([vm.tempAddEdge])
                      vm.tempAddEdge = null
                    },
                  })
                  // 加入创建指定类型连线的按钮
                  stypes.forEach((group) => {
                    group.items.forEach((toolItem) => {
                      menuItem.push({
                        title: toolItem.title,
                        action: async () => {
                          vm.createEdge(toolItem, addCell)
                        },
                      })
                    })
                  })
                  // 显示连线类型设置菜单
                  this.$ContextMenu.Show(
                    menuItem,
                    this.graph.lastEvent.x,
                    this.graph.lastEvent.y,
                    null,
                    this.graph
                  )
                }
                return
              }
            }
          })
        })

        //新增图元、连线完成后选中Cell显示属性清单
        this.graph.addListener(mxEvent.CELLS_ADDED, (sender, evt) => {
          this.$nextTick(() => {
            if (this.diagram?._isProcessing == true) {
              // 批处理状态下不切换选中项，否则会影响函数处理逻辑
            } else {
              this.graph.setSelectionCells(evt.properties.cells)
            }
          })
        })

        // 拖动节点的事件
        this.graph.addListener(mxEvent.CELLS_MOVED, (sender, evt) => {
          let cellsName = []
          this.$nextTick(() => {
            let resetCells = []
            let cells = evt.properties.cells
            let cellEdges = []
            cells.forEach((cell) => {
              if (cell.edges) {
                cellEdges = cellEdges.concat(cell.edges)
              }
            })
            cellEdges.forEach((_edge) => {
              let geo1 = null
              // 自连接，当元素拖曳时，同步调整位置
              if (_edge.source.id == _edge.target.id) {
                geo1 = _edge.geometry.clone()
                geo1.points[0].x += evt.properties.dx
                geo1.points[1].x += evt.properties.dx
                if (
                  vm.diagram.Stereotype == 'Sequence' &&
                  (_edge.Stereotype == 'Sequence' ||
                    _edge.Connector_Type == 'Sequence')
                ) {
                  // 时序图消息连线y坐标不变
                } else {
                  geo1.points[0].y += evt.properties.dy
                  geo1.points[1].y += evt.properties.dy
                }
              }
              // 非自联时序图消息连线，消息连线起点在右边，确保连线不折返
              else if (
                _edge.Stereotype == 'Sequence' ||
                _edge.Connector_Type == 'Sequence'
              ) {
                let points = [
                  _edge.source.geometry.x - _edge.source.geometry.width,
                  _edge.target.geometry.x - _edge.target.geometry.width,
                ]
                let x = Math.max(...points)
                if (_edge.geometry.points[0].x < x) {
                  if (!geo1) geo1 = _edge.geometry.clone()
                  geo1.points[0].x = x
                  geo1.points[1].x = x
                }
              }
              if (geo1) {
                resetCells.push({
                  cell: _edge,
                  geo1,
                })
              }
            })
            // TBD: 左右两侧的生命线，必须间成隔至少20，否则会导致消息连线堆叠；

            cells.forEach((cell) => {
              cell?.parent?.id.includes('group') && cellsName.push(cell.title)
              if (cell.CELLS_MOVED_CallBack) {
                cell.CELLS_MOVED_CallBack(cell)
              } else if (cell.Object_Type == 'Port') {
                // 当端口拖曳时，重新计算位置
                window.$DiagramUtils.PortRelocation(vm.diagram, cell, evt)
              } else {
                if (cell.parent.id == '1') {
                  return
                }
                if (cell.geometry.relative) {
                  // 相对定位情况的情况目前只有端口，后续如果用到相对定位，基于PortRelocation修改
                } else {
                  let canReset = false
                  // 如果元素移动到父元素之外，回调到父元素内
                  let geo1 = cell.geometry.clone()
                  if (cell.geometry.x < 0) {
                    geo1.x = 0
                    canReset = true
                  } else if (
                    cell.geometry.x + cell.geometry.width >
                    cell.parent.geometry.width
                  ) {
                    geo1.x = cell.parent.geometry.width - cell.geometry.width
                    canReset = true
                  }

                  if (cell.geometry.y < 0) {
                    geo1.y = 0
                    canReset = true
                  } else if (
                    cell.geometry.y + cell.geometry.height >
                    cell.parent.geometry.height
                  ) {
                    geo1.y = cell.parent.geometry.height - cell.geometry.height
                    canReset = true
                  }
                  if (canReset) {
                    resetCells.push({
                      cell,
                      geo1,
                    })
                  }
                }
              }
            })
            // 当时序图中的消息连线拖曳时，重新计算顺序；
            if (vm.diagram.Stereotype == 'Sequence') {
              window.$DiagramUtils.SequenceStyle.resetMessageSeqNo(vm.diagram)
            }
            if (resetCells.length > 0) {
              this.mxUndoManager.undo()
              graph.getModel().beginUpdate()
              orderEdges(graph)
              resetCells.forEach((item) => {
                graph.getModel().setGeometry(item.cell, item.geo1)
              })
              graph.getModel().endUpdate()
            } else {
              // 图元移动后，确保连线图层最高
              graph.getModel().beginUpdate()
              orderEdges(graph)
              graph.getModel().endUpdate()
            }
            if (vm.diagram.Stereotype == 'Sequence') {
              if (
                evt.properties.cells.some((_cell) => {
                  return _cell.Object_Type == 'Sequence'
                })
              ) {
                // 在时序图中，生命线移动时，重新计算位置
                window.$DiagramUtils.SequenceStyle.resetSequenceGeometry(
                  vm.diagram
                )
              }
            }
          })
        })

        // 尺寸变更
        this.graph.addListener(mxEvent.RESIZE_CELLS, (sender, evt) => {
          this.$nextTick(() => {
            let def = graph.getDefaultParent()
            if (evt.properties.cells.length > 0) {
              let cell = evt.properties.cells[0]
              // 当时序图中的永道尺寸变更时，重新计算尺寸；
              if (
                vm.diagram.Stereotype == 'Sequence' &&
                cell.Stereotype == 'Sequence'
              ) {
                window.$DiagramUtils.SequenceStyle.resetSequenceHeight(
                  vm.diagram,
                  cell
                )
              }
              let resetCells = []
              evt.properties.cells.forEach((cell) => {
                // 最外层元素不处理
                if (cell == def) {
                  return
                }
                if (cell.RESIZE_CELLS_CallBack) {
                  cell.RESIZE_CELLS_CallBack(cell)
                } else if (
                  cell.Object_Type == 'InteractionFragment' ||
                  cell.Object_Type == 'State'
                ) {
                  window.$DiagramUtils.ResizePartitions(vm.diagram, cell)
                }
                let geo1 = cell.geometry.clone()
                let canReset = false
                // 元素不得超出父元素
                if (cell.parent) {
                  if (cell.parent.geometry && geo1.x < 0) {
                    geo1.x = 0
                    canReset = true
                  }
                  if (cell.parent.geometry && geo1.y < 0) {
                    geo1.y = 0
                    canReset = true
                  }
                }
                // 确保端口的最小尺寸
                if (cell.Object_Type == 'Port') {
                  let SIZE_PORT_WIDTH = 15
                  let SIZE_PORT_HEIGHT = 15
                  if (cell.geometry.width < SIZE_PORT_WIDTH) {
                    geo1.width = SIZE_PORT_WIDTH
                    canReset = true
                  }
                  if (cell.geometry.height < SIZE_PORT_HEIGHT) {
                    geo1.height = SIZE_PORT_HEIGHT
                    canReset = true
                  }
                }
                // 确保类、实例、参数的最小尺寸；一些SysML模型的规则比较特殊暂不处理；
                else if (
                  cell.Object_Type == 'Class' ||
                  cell.Object_Type == 'Part' ||
                  cell.Object_Type == 'Interface'
                ) {
                  let SIZE_PART_WIDTH = 80
                  let SIZE_PART_HEIGHT = 30

                  // 按最右侧子元素计算父元素最小宽度，无子元素采用默认值
                  let findWidth = (cell) => {
                    return cell.geometry.width + cell.geometry.x
                  }
                  let child_x = cell.children
                    ?.sort((a, b) => {
                      return findWidth(b) - findWidth(a)
                    })
                    ?.find((element) => {
                      return !element.relative // 排除相对定位情况，这种情况一般是端口
                    })
                  if (child_x && cell.geometry.width < findWidth(child_x)) {
                    geo1.width = findWidth(child_x)
                    canReset = true
                  } else {
                    if (cell.geometry.width < SIZE_PART_WIDTH) {
                      geo1.width = SIZE_PART_WIDTH
                      canReset = true
                    }
                  }

                  // 按最下侧子元素计算父元素最小高度，无子元素采用默认值
                  let findHeight = (cell) => {
                    return cell.geometry.height + cell.geometry.y
                  }
                  let child_y = cell.children
                    ?.sort((a, b) => {
                      return findHeight(b) - findHeight(a)
                    })
                    ?.find((element) => {
                      return !element.relative // 排除相对定位情况，这种情况一般是端口
                    })
                  if (child_y && cell.geometry.height < findHeight(child_y)) {
                    geo1.height = findHeight(child_y)
                    canReset = true
                  } else {
                    if (cell.geometry.height < SIZE_PART_HEIGHT) {
                      geo1.height = SIZE_PART_HEIGHT
                      canReset = true
                    }
                  }
                }
                if (canReset) {
                  resetCells.push({
                    cell,
                    geo1,
                  })
                }
              })
              if (resetCells.length > 0) {
                this.mxUndoManager.undo()
                graph.getModel().beginUpdate()
                resetCells.forEach((item) => {
                  graph.model.setGeometry(item.cell, item.geo1)
                })
                graph.getModel().endUpdate()
              }
            }
          })
        })

        // 删除节点触发事件
        this.graph.addListener(mxEvent.CELLS_REMOVED, (sender, evt) => {
          this.$nextTick(() => {
            let removeCells = evt.properties.cells
            removeCells.forEach((item) => {
              if (item.CELLS_REMOVED_CallBack) {
                item.CELLS_REMOVED_CallBack(item)
              }
              // 拿每一个cellId在contextJsonData中进行遍历,并进行移除
              if (item.vertex) {
                // 判断是否为组节点
                if (item.isGroup) {
                  // this.$message.info(`移除了${item.id}组`);
                  this.contextJsonData['cells']['groups'].splice(
                    this.contextJsonData['cells']['groups'].findIndex(
                      (jsonItem) => {
                        return jsonItem.id == item.id
                      }
                    ),
                    1
                  )
                } else {
                  // this.$message.info(`移除${item.id}节点`);
                  this.contextJsonData['cells']['nodes'].splice(
                    this.contextJsonData['cells']['nodes'].findIndex(
                      (jsonItem) => {
                        return jsonItem.id == item.id
                      }
                    ),
                    1
                  )
                }
              } else if (item.edge) {
                // this.$message.info("移除了线");
                this.contextJsonData['edges'].splice(
                  this.contextJsonData['edges'].findIndex((jsonItem) => {
                    return jsonItem.id == item.id
                  }),
                  1
                )
              }
            })
            // 在时序图中，有生命线图元或消息连线被删除时，重新计算线的顺序
            if (vm.diagram.Stereotype == 'Sequence') {
              window.$DiagramUtils.SequenceStyle.resetMessageSeqNo(vm.diagram)
            }
          })
        })

        // 文本修改事件
        this.graph.addListener(mxEvent.LABEL_CHANGED, (sender, evt) => {
          if (evt.properties.cell) {
            window.$GlobalContext.RenameElement(
              evt.properties.cell,
              evt.properties.value
            )
          }
        })

        this.graph.graphHandler.shouldRemoveCellsFromParent = function (
          parent,
          cells,
          evt
        ) {
          // 所有子元素目前必须停靠在一个元素上
          return false
        }
      },
      // 配置鼠标事件
      configMouseEvent() {
        this.graph.addMouseListener({
          currentState: null,
          previousStyle: null,
          mouseDown: (sender, evt) => {
            let targetType = 'element'
            let target = {}
            if (!evt.state) {
              target = this.diagram
              targetType = 'diagram'
            } else if (evt.state.cell.edge) {
              target = evt.state.cell
              targetType = 'edge'
            } else {
              target = evt.state.cell
              targetType = 'element'
            }

            const time = Date.now()
            // 双击事件
            if (
              time - this.lastClickTime < 300 &&
              target.id == this.lastClickTarget.id
            ) {
              if (targetType == 'diagram') {
                //
              }
              if (targetType == 'element') {
                ShowElementRefDiagram(target.RealID)
              }
              if (targetType == 'edge') {
                //
              }
            }
            this.lastClickTarget = target
            this.lastClickTime = time
          },
          mouseMove: (sender, me) => {
            this.pageX = Math.ceil(me.evt.pageX)
            this.pageY = Math.ceil(me.evt.pageY)
            this.graphY = Math.ceil(me.graphY)
            this.$nextTick(function () {
              if (!this.diagram) return
              if (this.diagram.unsave != this.editor.modified) {
                this.diagram.unsave = this.editor.modified
              }
              // 调试发现，tabs不能及时更新label，只能手动调用$forceUpdate()
              this.$GlobalContext.diagramManager?.$forceUpdate()
            })
          },
          mouseUp: (sender, evt) => {
            let diagram = this.diagram
            if (!evt.state) {
              this.$store.commit('projectTreeSelectedItems', [diagram])
            }
            // 在时序图中，鼠标选中连线移动时，重新计算顺序
            if (diagram.Stereotype == 'Sequence') {
              window.$DiagramUtils.SequenceStyle.resetMessageSeqNo(diagram)
            }
          },
        })
      },
      //配置键盘事件
      configKeyEvent() {
        // 启动盘事件键，调试发现使用mxKeyHandler多次注册事件，不会导致事件重复
        let vm = this
        let getComp = () => {
          return vm.$store.getters['currentDiagram']?.diagramEditor
        }

        vm.keyHandler = vm.graphApp.getKeyHandler()

        // Ignores graph enabled state but not chromeless state
        vm.keyHandler.isEnabledForEvent = function (evt) {
          let dialogs = vm.$store.getters['dialogs']
          let diagramObjects =
            vm.$store.getters['currentSelectedDiagramObjects']
          let result = false // 标记键盘事件是否注册成功

          // Ctrl + Alt + 左方向键
          if (
            evt.altKey === true &&
            evt.ctrlKey === true &&
            evt.keyCode === 37
          ) {
            result = vm.portOrderAjustmentHandle(diagramObjects, 1) // 选中端口时
          }

          // Ctrl + Alt + 上方向键
          if (
            evt.altKey === true &&
            evt.ctrlKey === true &&
            evt.keyCode === 38
          ) {
            result = vm.portOrderAjustmentHandle(diagramObjects, 1) // 选中端口时
          }

          // Ctrl + Alt + 右方向键
          if (
            evt.altKey === true &&
            evt.ctrlKey === true &&
            evt.keyCode === 39
          ) {
            result = vm.portOrderAjustmentHandle(diagramObjects, 2) // 选中端口时
          }

          // Ctrl + Alt + 下方向键
          if (
            evt.altKey === true &&
            evt.ctrlKey === true &&
            evt.keyCode === 40
          ) {
            result = vm.portOrderAjustmentHandle(diagramObjects, 2) // 选中端口时
          }

          return result == true
            ? null
            : !mxEvent.isConsumed(evt) &&
                this.isGraphEvent(evt) &&
                this.isEnabled() &&
                (dialogs == null || dialogs < 1)
        }

        vm.keyHandler.bindControlKey(46, () => {
          // 删除
          var item = vm.$store.getters['currentSelectedDiagramObjects']
          if (item.length == 1) {
            if (item[0].vertex) {
              vm.$GlobalContext.DeleteElement(item[0])
            }
            if (item[0].edge) {
              vm.$GlobalContext.DeleteConnector(item[0])
            }
          }
        })
        vm.keyHandler.bindKey(46, () => {
          this.deleteSelectionCells()
        })
        vm.keyHandler.bindControlKey(83, () => {
          getComp()?.SaveToDiagram()
        })
        vm.keyHandler.bindControlKey(89, () => {
          getComp()?.goForward()
        })
        vm.keyHandler.bindControlKey(90, () => {
          getComp()?.goBack()
        })
        vm.keyHandler.bindControlKey(67, () => {
          getComp()?.copy()
        })
        vm.keyHandler.bindControlKey(88, () => {
          getComp()?.cut()
        })
        vm.keyHandler.bindControlKey(86, () => {
          getComp()?.paste()
        })

        // Ctrl+F
        vm.keyHandler.bindControlKey(70, (event) => {
          event.preventDefault()
          window.$GlobalContext.ShowView({
            // 标题
            tabLabel: '模型检索 ',
            // 唯一标识(不有空格、特殊符号)
            key: 'ModelRetrievalPage',
            // 视图组件的名称，用于获取vue组件
            view: 'ModelRetrievalPage',
            // 视图的data
            viewData: {},
          })
        })
      },
      //配置右键菜单栏
      configContextMenu() {

        let graph = this.graph

        let registerCustomDiagramAction = (menu, diagram)=>{
          // 加入自定义图的右键菜单
          window.$SmartDiagram.Actions.forEach((_action) => {

          if (_action.condition(diagram) && _action.smartmenu) {
            _action.smartmenu(diagram).forEach((_menuItem) => {
            
            //if (_menuItem.display && _menuItem.display() != true) {
            //  return
            //}
            console.log(_menuItem)
            menu.addItem(
              _menuItem.title,
              null,
              () => {
                try {
                  graph.model.beginUpdate()
                  _menuItem.action()
                } finally {
                  graph.model.endUpdate()
                }
              },
              null,
              null,
              _menuItem.enable()
            )
          })
          menu.addSeparator(null, true)
        }
        })
      }


      
      let registerCustomCellAction = (menu, diagram, cells)=>{
         // 加入自定义图元的右键菜单
         window.$SmartCell.Actions.forEach((_action) => {

        
            if (_action.condition(diagram, cells) && _action.smartmenu) {
              _action.smartmenu(diagram, cells).forEach((_menuItem) => {
            //   if (_menuItem.display && _menuItem.display() != true) {
            //     return
            //   }
            console.log(_menuItem)
                menu.addItem(
                  _menuItem.title,
                  null,
                  () => {
                    try {
                      graph.model.beginUpdate()
                      _menuItem.action()
                    } finally {
                      graph.model.endUpdate()
                    }
                  },
                  null,
                  null,
                  _menuItem.enable()
                )
              })
              menu.addSeparator(null, true)
            }
            })
      }

        // 禁用浏览器默认的右键菜单栏
        mxEvent.disableContextMenu(this.$refs.container)
        this.graph.popupMenuHandler.factoryMethod = (menu, cell, event) => {
          // 隐藏当前打开的右键菜单
          this.$GlobalContext.HideContextMenu()
          let cells = this.graph.selectionModel.cells
          let vm = this
          let roList = cells.map((x) => x.Readonly)
          // 选中项只读、图只读，菜单不可用
          let enable =
            [true, 'true'].includes(this.diagram.Readonly) ||
            roList.includes('true') ||
            roList.includes(true)
              ? false
              : true // 菜单是否可用
          if (enable) {
            // 图编辑锁定，不可使用
            enable = vm.$store.getters['currentDiagram'].disableEditor != true
          }

          /** 图的右键菜单 */
          if (cells.length == 0) {
            /** 在画布中右键未选择任何元素时，加载图Addin菜单 */
            this.loadDiagramMenu(menu, this.diagram)
            
            // 加入自定义图的右键菜单
            registerCustomDiagramAction(menu, this.diagram)
            return
          } else if (cells.length == 1) {
          /** 单个元素的右键菜单 */
            /** 在画布中右键选择一个元素时，加载元素Addin菜单 */
            var _cell = cells[0]
            this.loadDiagramMenu(menu, _cell)
            registerCustomCellAction(menu, this.diagram, cells)
            // 设置连线默认菜单
            if (_cell.edge) {
              // 设置连线样式菜单
              let edgeStyleMenu = (menu, edges) => {
                // 连线功能
                let edgeStyle = menu.addItem(
                  '连线样式',
                  null,
                  null,
                  null,
                  null,
                  enable
                )
                menu.addItem(
                  '设为直线',
                  null,
                  () => {
                    this.setPolyline(graph, edges, 0)
                  },
                  edgeStyle,
                  null,
                  enable
                )
                menu.addItem(
                  '设为折线-水平',
                  null,
                  () => {
                    this.setPolyline(graph, edges, 1)
                  },
                  edgeStyle,
                  null,
                  enable
                )
                menu.addItem(
                  '设为折线-垂直',
                  null,
                  () => {
                    this.setPolyline(graph, edges, 2)
                  },
                  edgeStyle,
                  null,
                  enable
                )
              }
              edgeStyleMenu(menu, [_cell])
            }
          }
          /** 在画布中右键选择元素时 */
          if (cells.length > 1) {
            /** 在画布中右键选择多个元素时，加载多元素Addin菜单 */
            this.loadDiagramMenu(menu, cells)

            // 加入自定义图元的右键菜单
            registerCustomCellAction(menu, this.diagram, cells)
          

            // 设置块默认菜单
            let _vertexs = cells.filter((item) => {
              if (item.type == 'Element') {
                return true
              }
            })

            // 不允许从临时画布Graph中删除元素
            if (_vertexs?.length > 0) {
              if (vm.diagram.isGraph != true && _vertexs?.length > 1) {
                menu.addItem(
                  '删除元素',
                  null,
                  () => {
                    this.deleteBatchSelectionCells(cells)
                  },
                  null,
                  null,
                  enable
                )
              }
              if (vm.diagram.exDATA2 == 'MTBFBlockDesign') {
                //
              } else {
                menu.addItem(
                  '从图中删除',
                  null,
                  () => {
                    this.deleteSelectionCells()
                  },
                  null,
                  null,
                  enable
                )
              }
            }

            // 设置连线默认菜单
            let _edges = cells.filter((item) => {
              if (item.edge) {
                return true
              }
            })
            if (_edges?.length > 0) {
              // 设置连线样式菜单
              let edgeStyleMenu = (menu, edges) => {
                // 连线功能
                let edgeStyle = menu.addItem(
                  '连线样式',
                  null,
                  null,
                  null,
                  null,
                  enable
                )
                menu.addItem(
                  '设为直线',
                  null,
                  () => {
                    this.setPolyline(graph, edges, 0)
                  },
                  edgeStyle,
                  null,
                  enable
                )
                menu.addItem(
                  '设为折线-水平',
                  null,
                  () => {
                    this.setPolyline(graph, edges, 1)
                  },
                  edgeStyle,
                  null,
                  enable
                )
                menu.addItem(
                  '设为折线-垂直',
                  null,
                  () => {
                    this.setPolyline(graph, edges, 2)
                  },
                  edgeStyle,
                  null,
                  enable
                )
              }
              edgeStyleMenu(menu, _edges)
            }
          }
        }
        // Hides context menu
        mxEvent.addGestureListeners(
          document,
          mxUtils.bind(this, function () {
            this.graph.popupMenuHandler.hideMenu()
            if (this.tempAddEdge) {
              this.graph.removeCells([this.tempAddEdge])
              this.tempAddEdge = null
            }
          })
        )
      },
      // 删除节点
      deleteSelectionCells() {
        let cells = this.graph.getSelectionCells()
        // 选中元素中包含只读的，不可以删除
        if (
          cells.find((cell) => cell.Readonly == true || cell.Readonly == 'true')
        ) {
          return
        }
        let removes = [...cells]
        cells.forEach((element) => {
          if (element.edges) {
            removes.push(...element.edges)
          }
        })
        this.graph.removeCells(removes)
      },

      /**从工程库中删除图元 */
      deleteBatchSelectionCells(item) {
        this.$GlobalContext.DeleteCells(item)
      },

      /**
       * 加载Addin图右键菜单
       * @param {*} element 选中的元素（可多个）
       */
      loadDiagramMenu(menu, element) {
        let vm = this
        let getEnable = (menuItem) => {
          let enable = [null, undefined].includes(menuItem.enable)
            ? true
            : menuItem.enable(element) // 判断当前才是是否启用
          // 必须显示的菜单名称
          if (MENU_MUST.includes(menuItem.key) != true && enable) {
            // 当图不可编辑时，锁定右键菜单
            enable = vm.$store.getters['currentDiagram'].disableEditor != true
          }
        }
        let checkMenu = (menuItem) => {
          return (
            menuItem.condition &&
            menuItem.condition(element, 'diagramEditor', this) == true
          )
        }
        let addMenuItem = (menuItems, parentMenu, isTopMenu) => {
          // 分组，组之间加入分割线
          const groupcollection = Object.groupBy(
            menuItems,
            ({ group }) => group
          )
          Object.keys(groupcollection).forEach((groupkey) => {
            // 获取当前组中可用菜单
            let meuns = groupcollection[groupkey].filter((x) => checkMenu(x))
            if (meuns?.length > 0) {
              meuns.forEach(async (item) => {
                // 获取可用子菜单
                let subMenu = item.children?.filter((x) => checkMenu(x)) ?? []
                if (subMenu.length > 0) {
                  // 加入无事件父级菜单
                  let pmenu = menu.addItem(
                    item.title,
                    null,
                    () => {},
                    parentMenu,
                    item.iconCls,
                    getEnable(item)
                  )
                  if (item.action) {
                    // 因为调试时发现在分组情况下，父菜单的点击UI存在缺陷，所以在加入子菜单之前加入一个菜单
                    menu.addItem(
                      item.title,
                      null,
                      () => {
                        item.action(this, element, 'diagramEditor')
                      },
                      pmenu,
                      null,
                      getEnable(item)
                    )
                    menu.addSeparator(pmenu, true)
                  }
                  // 加入子菜单
                  addMenuItem(subMenu, pmenu)
                } else {
                  // 忽略无事件的菜单
                  if (item.action) {
                    menu.addItem(
                      item.title,
                      null,
                      () => {
                        item.action(this, element, 'diagramEditor')
                      },
                      parentMenu,
                      item.iconCls,
                      getEnable(item)
                    )
                  }
                }
              })
              // 仅顶层菜单，在每组之间加分割线
              if (isTopMenu == true) {
                menu.addSeparator(parentMenu, true)
              }
            }
          })
        }
        addMenuItem(this.$ContextMenuManager.ContextMenuItems, menu, true)
      },
      /**
       * 创建连线（元素之间的连接关系）
       * @param {*} data 当前连线基础数据
       * @param {*} addCell 待创建连线关系的元素（包含源元素和目标元素）
       */
      createEdge(data, addCell) {
        let vm = this
        /** 连线时有菜单数据时 */
        if (
          ![null, undefined].includes(data.menuItem) &&
          data.menuItem.length > 0
        ) {
          let menuItem = data.menuItem // 菜单数据
          let setMenuItemAction = (item) => {
            item.forEach((mi) => {
              /** 菜单选择处理事件，有子菜单不应有事件处理赋值为NULL */
              let meunSelectedEvent =
                ![null, undefined].includes(mi.menuItem) &&
                mi.menuItem.length > 0
                  ? null
                  : () => {
                      this.createConnection(mi, addCell)
                    }
              vm.$set(mi, 'action', meunSelectedEvent)
              // 如果当前有子菜单进行递归
              if (
                ![null, undefined].includes(mi.menuItem) &&
                mi.menuItem.length > 0
              ) {
                setMenuItemAction(mi.menuItem)
              }
            })
          }
          setMenuItemAction(menuItem)
          window.$ContextMenu.Show(
            menuItem,
            this.graph.lastEvent.x,
            this.graph.lastEvent.y,
            null,
            this.graph
          )
        } else {
          /** 无菜单时，直接创建连线 */
          this.createConnection(data, addCell)
        }
      },
      /**
       * 创建连线关系
       * @param {*} data 当前连线基础数据
       * @param {*} addCell 待创建连线关系的元素（包含源元素和目标元素）
       */
      createConnection(data, addCell) {
        let isNotNUll = ['', null, undefined]
        if (isNotNUll.includes(this.graph) || isNotNUll.includes(data)) return

        this.graph.getModel().beginUpdate()
        try {
          let sourceCell = addCell.source
          let targetCell = addCell.target

          // 检查创建连接时是否需要同时创建端口。
          // 如果需要，则将当前创建的端口作为连线的源元素和目标元素。
          if (!isNotNUll.includes(data.sourcePort)) {
            let sourceX = sourceCell.geometry.x
            let sourceY = sourceCell.geometry.y
            sourceCell = this.$GlobalContext.toolbox.addElementTheCanvas(
              this.graph,
              sourceCell,
              data.sourcePort,
              sourceX,
              sourceY
            )
          }
          if (!isNotNUll.includes(data.targetPort)) {
            let targetX = targetCell.geometry.x
            let targetY = targetCell.geometry.y
            targetCell = this.$GlobalContext.toolbox.addElementTheCanvas(
              this.graph,
              targetCell,
              data.targetPort,
              targetX,
              targetY
            )
          }

          let style =
            typeof data.style === 'object'
              ? Object.keys(data.style)
                  .map((attr) =>
                    attr === 'fillColor' ? '' : `${attr}=${data.style[attr]};`
                  )
                  .join('')
              : data.style.slice(0, -1)

          let edge = this.graph.createEdge(
            this.graph.getDefaultParent(),
            uuid(),
            null,
            null,
            style
          )
          edge.ea_guid = `{${uuid()}}`
          edge.style = addCell.style + style
          edge.Stereotype = data.Stereotype
          edge.value = data.value ?? data.title
          edge.title = data.title
          edge.Connector_Type = data.Connector_Type
          edge.PDATA1 = data.PDATA1
          edge.PDATA2 = data.PDATA2
          edge.PDATA3 = data.PDATA3
          edge.PDATA4 = data.PDATA4
          edge.PDATA5 = data.PDATA5
          if (
            this.diagram.Stereotype == 'Sequence' &&
            data.Connector_Type == 'Sequence'
          ) {
            edge.style = mxUtils.setStyle(
              edge.style,
              'edgeStyle',
              'orthogonalEdgeStyle'
            )
            edge.style = mxUtils.setStyle(edge.style, 'rounded', '0')
            edge.style = mxUtils.setStyle(edge.style, 'jettySize', 'auto')
            edge.style = mxUtils.setStyle(edge.style, 'orthogonalLoop', '1')
            edge.geometry.relative = 1
            edge.geometry.x = 0
            edge.geometry.y = 0
            // 添加点
            let y1 = sourceCell.geometry.y + sourceCell.geometry.height / 2
            if (sourceCell.id == targetCell.id) {
              let x1 = sourceCell.geometry.x + sourceCell.geometry.width + 10
              edge.geometry.points = [
                new mxPoint(x1, y1),
                new mxPoint(x1, y1 + 30),
              ]
            } else {
              // 消息连线是自动曲折的直角折线
              let exitX = sourceCell.geometry.x + sourceCell.geometry.width / 2
              let x1 =
                (targetCell.geometry.x - sourceCell.geometry.x) / 2 + exitX
              edge.geometry.points = [new mxPoint(x1, y1), new mxPoint(x1, y1)]
            }
          }
          this.graph.addEdge(
            edge,
            this.graph.getDefaultParent(),
            sourceCell,
            targetCell
          )
          this.saveToDiagramConnector(
            edge,
            sourceCell.RealID,
            targetCell.RealID
          ) // 保存连线关系

          edge['SourceID'] = sourceCell.id
          edge['TargetID'] = targetCell.id
          this.contextJsonData['edges'].push(edge)

          // 移除预览连线
          this.graph.removeCells([this.tempAddEdge])
          this.tempAddEdge = null
          this.graph.getModel().endUpdate()
        } catch (e) {
          console.error(e)
        }
      },
      /**
       * 保存连线关系
       */
      saveToDiagramConnector(edge, startObjectId, endObjectId) {
        /** 保存连线关系 */
        let conn = Object.assign({}, edge)
        delete conn.parent
        delete conn.source
        delete conn.target
        let paramData = {
          cell: conn,
          diagramId: this.diagram.RealID,
          startObjectId: startObjectId,
          endObjectId: endObjectId,
        }
        SaveToDiagramConnector(paramData).then((data) => {
          edge['RealID'] = parseInt(data.RealID)
          edge['type'] = data.type
        })
      },
      // 前进
      goForward() {
        this.mxUndoManager.redo()
      },
      // 撤退
      goBack() {
        this.mxUndoManager.undo()
      },
      // 放大
      zoomIn() {
        this.graph.zoomIn()
        this.dynamicScale = (this.graph.view.scale * 100).toFixed(0) //更新dynamicScale值
      },
      // 缩小
      zoomOut() {
        this.graph.zoomOut()
        this.dynamicScale = (this.graph.view.scale * 100).toFixed(0) //更新dynamicScale值
      },
      // 等比例缩放
      autoSize() {
        this.graph.zoomActual()
        this.dynamicScale = (this.graph.view.scale * 100).toFixed(0) //更新dynamicScale值
        this.graph.fit() //自适应
      },
      // 全不选
      clearSelection() {
        this.graph.clearSelection()
      },
      //复制
      copy() {
        if (this.diagram.demonstration == true) {
          this.$message.info('演示模式状态下不能复制粘贴，请在刷新图后重试。')
          return
        }
        let selectionCells = this.graph.getSelectionCells()
        mxClipboard.copy(this.graph, selectionCells)
      },
      //粘贴
      async paste() {
        if (mxClipboard?.getCells()?.length > 0 == false) {
          return // 无可粘贴元素
        }
        if (this.diagram.demonstration == true) {
          this.$message.info('演示模式状态下不能复制粘贴，请在刷新图后重试。')
          return
        }
        let graph = this.graph
        let delta = mxClipboard.insertCount * mxClipboard.STEPSIZE
        let adds = []
        if (this.diagram.isGraph) {
          // 粘贴，忽略模型关系
          adds = mxClipboard.paste(this.graph)
        } else {
          // 粘贴，计算模型关系
          let getCellMxId = (cell) => {
            if (cell.RealID && cell.vertex) {
              return `o_${cell.RealID}`
            } else if (cell.RealID && cell.edge) {
              return `c_${cell.RealID}`
            } else {
              return cell.id
            }
          }
          let setCellMxId = (cells) => {
            cells.forEach((cell) => {
              if (cell) {
                // 重置id，更新model避免脏数据
                let canReset = false
                if (graph.model.cells[cell.id]) {
                  delete graph.model.cells[cell.id]
                  canReset = true
                }
                cell.setId(getCellMxId(cell))
                if (canReset) {
                  graph.model.cells[cell.id] = cell
                }
              }
              if (cell?.children) {
                setCellMxId(cell.children)
              }
            })
          }

          let pasetCells = []
          let pasteCellDict = {}
          let existsCells = []
          // 父节点在图中已存在，需要回写parent的数据集，key:子元素id(复制的),value:父节点cell(已存在)
          let resetParentDict = {}
          // 子节点在图中已存在，需要回写children的数据集，key:子元素id(已存在),value:父节点cell(复制的)
          let resetChildrenDict = {}
          // 检查是否需要粘贴
          let checkEexists = (cells) => {
            let checkChil = true
            cells.forEach((_cell) => {
              try {
                if (_cell.type == 'Label') {
                  return // Label 忽略检查
                }
                let id = getCellMxId(_cell)
                let existsCell = graph.model.getCell(id)
                if (existsCell) {
                  // 忽略图中已存在的元素
                  existsCells.push(existsCell)
                  // 从元素的子节中移除
                  if (_cell.parent) {
                    _cell.removeFromParent()
                  }
                  return
                }
                if (_cell.parent?.RealID > 0) {
                  let pid = getCellMxId(_cell.parent)
                  // 当前节点是图中元素的子节点，需要重写parent
                  let parentCell = graph.model.getCell(pid)
                  if (parentCell) {
                    resetParentDict[id] = { _cell: _cell, parent: parentCell }
                  }
                }
                // 检查当前元素是否是图中元素的父节点
                {
                  for (var _id in graph.model.cells) {
                    let child = graph.model.cells[_id]
                    if (child.vertex && child.Parent_ID == _cell.RealID) {
                      resetChildrenDict[_id] = { id: id, child: child }
                    }
                  }
                }
                {
                  pasetCells.push(_cell)
                  pasteCellDict[_cell.RealID] = _cell
                }
              } finally {
                // 检查子元素
                if (checkChil && _cell.children) {
                  checkEexists(_cell.children)
                }
              }
            })
          }
          checkEexists(mxClipboard.getCells())
          // 粘贴元素，并保留样式，粘贴完成后重写id，因为importCells接口会生成随机值作为id，不符合业务规则；
          let newCells = graph.importCells(
            pasetCells,
            delta,
            delta,
            graph.getDefaultParent()
          )
          setCellMxId(newCells)
          graph.getModel().beginUpdate()
          // 回写parent
          if (resetParentDict) {
            Object.keys(resetParentDict).map((id) => {
              let cell = graph.model.getCell(id)
              let parent = resetParentDict[id].parent
              if (!parent.children) {
                parent['children'] = []
              }
              parent.children.push(cell)
              let _cell = resetParentDict[id]._cell
              cell.setGeometry(_cell.geometry)
              cell.removeFromParent()
              cell.setParent(parent)
            })
          }
          // 回写children
          if (resetChildrenDict) {
            Object.keys(resetChildrenDict).map((id) => {
              let child = resetChildrenDict[id].child
              let cell = graph.model.getCell(resetChildrenDict[id].id)
              if (!cell.children) {
                cell['children'] = []
              }
              cell.children.push(child)
              child.removeFromParent()
              child.setParent(cell)
            })
          }
          graph.getModel().endUpdate()
          adds = [...newCells]
        }
        // 选中粘贴的元素，滚动到粘贴的元素
        graph.orderCells(false, adds)
        graph.setSelectionCells(adds)
        graph.scrollCellToVisible(adds, true)
      },
      //剪切
      cut() {
        let cells = []
        cells = this.graph.getSelectionCells()
        mxClipboard.cut(this.graph, cells)
      },
      // 复制到图
      copyAsImage() {
        let graph = this.graph
        // 移除style属性中的image属性，否则影响保存图片
        for (var _key in graph.model.cells) {
          let _cell = graph.model.cells[_key]
          let _style = _cell.style
          _style = mxUtils.setStyle(_style, 'image', null)
          graph.model.setStyle(_cell, _style)
        }
        let cells = mxUtils.sortCells(
          graph.model.getTopmostCells(graph.getSelectionCells())
        )
        let xml = mxUtils.getXml(
          cells.length == 0
            ? this.editor.getGraphXml()
            : graph.encodeCells(cells)
        )
        this.graphApp.copyImage(cells, xml)
      },
      // 导入xml文件后更新视图
      async uploadPaintFlow(xml) {
        xml = xml.replace(/\r\n/g, '')
        await this.decode(xml, this.graph)
      },
      createXmlDom(str) {
        if (document.all) {
          //判断浏览器是否是IE
          let xmlDom = new ActiveXObject('Microsoft.XMLDOM')
          xmlDom.loadXML(str)
          return xmlDom
        } else {
          return new DOMParser().parseFromString(str, 'text/xml')
        }
      },
      // 渲染xml流程图
      async decode(xml, graph) {
        this.graph.getModel().beginUpdate()
        try {
          let xmlDoc = this.createXmlDom(xml)
          let node = xmlDoc.documentElement
          let dec = new mxCodec(node.ownerDocument)
          dec.decode(node, graph.getModel())
          if (this.diagram.Stereotype == 'Sequence') {
            await this.$DiagramUtils.SequenceStyle.updateSequenceDiagramStyle(
              this.diagram
            )
            this.setUndo()
            this.$set(this.diagram, 'unsave', false)
            this.editor.modified = false
          } else if (this.diagram.Stereotype == 'Statechart') {
            await this.$DiagramUtils.StatechartStyle.Restyle(this.diagram)
          } else if (this.diagram.Stereotype == 'IAML Architecture: DataFlow') {
            // 获取数据流配置
            await GetFlowSequenceSetting(
              this.diagram,
              this.diagram.Attitude?.Functional
            )
          }
        } finally {
          this.graph.getModel().endUpdate()
          this.setUndo()
          this.editor.modified = false
          // 还原尺寸
          this.graph.zoomTo(this.tempGraphScale)
          // 还原选中项
          let cells = []
          this.tempSelectedDiagramObjectIds.forEach((id) => {
            cells.push(this.graph.model.getCell(id))
          })
          this.graph.setSelectionCells(cells)
          if (this.diagram?.OpenedAction) {
            await this.diagram.OpenedAction()
          }
          if (this.diagram?.ShowNextDiagram) {
            await this.diagram.ShowNextDiagram()
            delete this.diagram.ShowNextDiagram
          }
        }
      },
      // 准备撤销还原功能
      setUndo() {
        // 先移除再构建
        this.graph.getModel().removeListener(mxEvent.UNDO)
        this.graph.getView().removeListener(mxEvent.UNDO)
        // 构造具有给定历史记录大小的新撤消管理器。默认100步
        this.mxUndoManager = new mxUndoManager()
        let listener = (sender, evt) => {
          this.mxUndoManager.undoableEditHappened(evt.getProperty('edit'))
        }
        this.graph.getModel().addListener(mxEvent.UNDO, listener)
        this.graph.getView().addListener(mxEvent.UNDO, listener)
        // undo和编辑，是两回事，分别处理；
        // this.editor.modified = false
      },
      // 导出xml数据
      encodeXml() {
        const encoder = new mxCodec()
        const result = encoder.encode(this.graph.getModel())
        {
          // 自定义图不依赖与diagramObject，所有需要计算id，保存到xml中。
          // 由于mxObjectCodec.prototype.idrefs和exclude不生效，需要手动计算
          var mxCells = result.getElementsByTagName('root')[0].childNodes
          var graph = this.graph
          mxCells.forEach((xmlCell) => {
            let edge = xmlCell.attributes['edge']?.textContent
            let vertex = xmlCell.attributes['vertex']?.textContent
            // xml数据转换，idrefs是需要将对象替换为id的场景，exclude是忽略的场景
            function reset(xmlCell, idrefs, exclude) {
              if (idrefs?.length > 0) {
                idrefs.forEach((_key) => {
                  let text = xmlCell.attributes[_key]?.textContent
                  if (!text) {
                    let id = xmlCell.attributes['id'].textContent
                    let cell = graph.model.getCell(id)
                    if (cell && cell[_key]?.id) {
                      xmlCell.setAttribute(_key, cell[_key].id)
                    }
                  }
                })
              }
              if (exclude?.length > 0) {
                exclude.forEach((_key) => {
                  let _attr = xmlCell.attributes[_key]
                  if (_attr) {
                    xmlCell.RemoveAttribute(_key)
                  }
                })
              }
              // 删除Owner节点，因为一个图中的若干元素属于同一个设计单元时，Owner会重复出现的，Owner具备id属性，在mxGraph解析时会报id重复错误；
              let canRemoveNodes = ['Owner']
              let objectNodes = xmlCell.getElementsByTagName('Object')
              // 遍历这些节点, 从后往前遍历以避免索引问题
              for (let i = objectNodes.length - 1; i >= 0; i--) {
                // 检查 'as' 属性是否为 'Owner'，如果是，则删除该节点
                const objectNode = objectNodes[i]
                const _as = objectNode.getAttribute('as')
                if (_as && canRemoveNodes.includes(_as)) {
                  xmlCell.removeChild(objectNode)
                }
              }
            }
            if (vertex) {
              reset(
                xmlCell,
                ['parent'],
                ['CELLS_REMOVED_CallBack', 'RESIZE_CELLS_CallBack']
              )
            } else if (edge) {
              reset(xmlCell, ['parent', 'source', 'target'])
            }
          })
        }
        let xml = mxUtils.getPrettyXml(result)
        return xml
      },
      // 鼠标滚轮事件
      handleScroll(event) {
        // 按住Ctrl时，允许滚轮缩放图片
        if (!event.ctrlKey) {
          return
        }
        if (event.wheelDelta > 0) {
          this.zoomIn()
        } else {
          this.zoomOut()
        }
        event.preventDefault()
      },
      async AutoLayout() {
        await window.$DiagramUtils.CheckDiagramIsSaved(
          this.diagram,
          async () => {
            this.$showLoading({
              text: '排布中...',
            })
            const data = await LayoutDiagram({ diagramId: this.diagramId })
            this.ShowDiagram().finally(() => {
              let allCells = this.graph.model.root?.children[0]?.children
              let edgeList = allCells.filter((x) => x.edge == 1)
              if (['TB', 'BT'].includes(data)) {
                this.setPolyline(this.graph, edgeList, 2)
              } else if (['LR', 'RL'].includes(data)) {
                this.setPolyline(this.graph, edgeList, 1)
              }
            })
            this.$hideLoading()
          }
        )
      },
      async SetStereotypeVisible() {
        SetStereotypeVisible({
          diagramId: this.diagramId,
          isDisplay: !this.diagram?.StereotypeVisible,
        }).then(async () => {
          this.ReloadDiagram()
        })
      },
      /**
       * 刷新图
       * @param ignoreChange 是否忽略图已修改的提示
       */
      async ReloadDiagram(ignoreChange = false) {
        if (ignoreChange) {
          this.ShowDiagram()
        } else {
          // 当图未编辑时，重新渲染图
          await window.$DiagramUtils.CheckDiagramIsSaved(
            this.diagram,
            this.ShowDiagram,
            () => {},
            () => {}
          )
        }
      },
      /** 展示图 */
      async ShowDiagram() {
        this.graph['diagram'] = this.diagramId
        this.graph['uuid'] = uuid()
        var vm = this
        vm.$showLoading({
          text: '渲染中...',
        })
        this.graph.setEnabled(true)
        this.$set(this.diagram, 'unsave', false)
        this.$set(this.diagram, 'locking', false)
        this.$set(this.diagram, 'demonstration', false)
        // 保存当前缩放比例
        this.tempGraphScale = this.graph.view.scale
        if (vm.diagram) {
          // 临时保存选中，在刷新图之后重新选中
          if (vm.diagram.RealID == vm.$store.getters['currentDiagram'].RealID) {
            vm.tempSelectedDiagramObjectIds = []
            if (vm.$store.getters['currentSelectedDiagramObjects']) {
              vm.$store.getters['currentSelectedDiagramObjects'].forEach(
                (element) => {
                  vm.tempSelectedDiagramObjectIds.push(element.id)
                }
              )
            }
          }

          if (!vm.diagram.subsystem) {
            vm.diagram['subsystem'] = await GetSubsystem(
              vm.diagram.RealID,
              vm.diagram.type
            )
          }
        }
        if (vm.diagramId > 0) {
          await GetMxGraphXml({
            diagramId: vm.diagramId,
          })
            .then(async (res) => {
              vm.$set(vm.diagram, 'isLockEditing', res.isLockEditing)
              vm.$set(vm.diagram, 'disableEditor', res.disableEditor)
              vm.$set(vm.diagram, 'EditingUserName', res.EditingUserName)
              vm.$set(vm.diagram, 'ModifiedDate', res.ModifiedDate)
              // 额外属性，以json字符串形式保存
              if (res.Attitude) {
                vm.$set(vm.diagram, 'Attitude', JSON.parse(res.Attitude))
              }
              vm.$set(vm.diagram, 'StereotypeVisible', res.StereotypeVisible)
              if (res.isLockEditing == true) {
                // 非本人锁定图编辑权限，此图不可编辑
                vm.graph.setEnabled(res.disableEditor != true)
              }
              vm.ModifiedDate = res.ModifiedDate
              // if (res.isGraph) {
              //     vm.uploadPaintFlow(res.GraphXML)
              // }else{
              //     vm.uploadPaintFlow(res.Xml)
              // }
              await vm.uploadPaintFlow(res.Xml)
              vm.autoSave = true
              //vm.startAutoSave()
            })
            .catch((err) => {
              vm.$hideLoading()
              vm.$confirm(err, '错误', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'error', // ‘success’(成功) /warning(警告)/info(消息)/error(错误)/;
                showCancelButton: false, //是否显示取消按钮
                showClose: false, //是否显示右上角的x
                closeOnClickModal: false, //是否可以点击空白处关闭弹窗
              })
              // 渲染失败，关闭diagram选项卡
              vm.$GlobalContext.CloseDiagramByRealID(vm.diagramId)
            })
            .then(() => {
              vm.$hideLoading()
            })
        } else if (vm.diagram.xml) {
          await vm.uploadPaintFlow(vm.diagram.xml)
          vm.$hideLoading()
        } else {
          // 显示空白图
          vm.$hideLoading()
        }
      },
      startAutoSave() {
        if (this.autoSave) {
          const that = this
          setTimeout(() => {
            if (!that.autoSave) return
            if (that.diagram.locking == true) return
            if (that.diagram.demonstration == true) return
            if (that.diagram.unsave) that.SaveToDiagram()
            that.startAutoSave()
          }, 300000) // 每5分钟自动保存
        }
      },
      /** 保存当前图 */
      async SaveToDiagram() {
        if (this.diagram.unsave == false) {
          console.log('*当前图未修改，不执行保存')
          return
        }

        if (this.tempAddEdge) {
          this.graph.removeCells([this.tempAddEdge])
          this.tempAddEdge = null
        }
        if (this.diagram.locking == true) return
        if (this.diagram.demonstration == true) {
          this.$message.info('演示模式状态下不能保存，请在刷新图后重试。')
          return
        }
        if (this.diagramId <= 0) {
          // 临时图，通过XML保存
          this.$showDialog(UploadDiagramXML, {
            uploadDataVisible: true,
            graphXml: this.encodeXml().toString(),
            isOutputXml: true,
          })
          return
        }
        let properties = []
        this.$GlobalContext.tempProps.forEach((item) => {
          let keyvalues = []
          item.properties.forEach((r) => {
            if (!r.isgroup1 && !r.isgroup2) {
              keyvalues.push({ name: r.property, val: r.Val })
            }
          })
          properties.push({ ea_guid: item.ea_guid, keyvalues: keyvalues })
        })
        var vm = this
        if (vm.loading == true) {
          return
        }
        vm.loading = true
        vm.$showLoading({
          text: '保存中...',
        })
        // 执行自定义图事件
        await window.$SmartDiagram.Execute(vm.diagram, 'SaveToDiagram')
        // 保存图数据到后台
        await SaveToDiagram({
          isGraph: vm.diagram.isGraph,
          diagramId: vm.diagramId,
          xml: vm.encodeXml().toString(),
          properties: properties,
          ModifiedDate: vm.ModifiedDate,
        })
          .then((data) => {
            vm.$set(vm.diagram, 'unsave', false)
            vm.$message.success('图保存成功')
            vm.ModifiedDate = data.ModifiedDate
            vm.setUndo()
            vm.editor.modified = false
            var random = Math.random()
            vm.$store.commit('reloadCurrentDiagramRandom', random)
            // 保存后刷新一次
            vm.ShowDiagram()
            vm.$GlobalContext.tempProps = []
          })
          .catch((err) => {
            vm.$GlobalContext.messageBox.error(err)
          })
          .finally(() => {
            vm.$hideLoading()
            setTimeout(() => {
              vm.loading = false
            }, 300)
          })
      },
      async downLoadImage() {
        let vm = this
        let graph = vm.$store.getters['currentGraph']
        let diagram = vm.$store.getters['currentDiagram']
        let diagramDetail =
          '作者:' +
          diagram.Author +
          '\n' +
          '名称:' +
          diagram.RealName +
          '\n' +
          '类型:' +
          diagram.Stereotype +
          '\n' +
          '所属:' +
          diagram.Owner?.RealName +
          '\n' +
          '修改时间:' +
          diagram.ModifiedDate +
          '\n'
        if (!graph) {
          this.$message.error('请先打开一个图')
          return
        }
        await GetMxGraphImage({
          diagramId: graph.diagram,
          diagramDetail: diagramDetail,
        })
          .then((res) => {
            vm.saveBas64Img(res, diagram.RealName)
          })
          .catch((err) => {
            vm.$GlobalContext.messageBox.error(err)
          })
      },
      saveBas64Img(base64, name) {
        let byteCharacters = atob(
          base64.replace(/^data:image\/(png|jpeg|jpg);base64,/, '')
        )
        let byteNumbers = new Array(byteCharacters.length)
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i)
        }
        let byteArray = new Uint8Array(byteNumbers)
        let blob = new Blob([byteArray], {
          type: undefined,
        })
        let aLink = document.createElement('a')
        aLink.download = `${name}.png` //这里写保存时的图片名称
        aLink.href = URL.createObjectURL(blob)
        aLink.click()
      },
      /**
       * 端口的顺序处理事件
       * @param {*} direction 方向 1:向左\向上 2:向右向下
       */
      portOrderAjustmentHandle(diagramObjects, direction) {
        let result = false
        result =
          ![null, undefined].includes(diagramObjects) &&
          diagramObjects.length > 0

        let dObj = diagramObjects[0]
        result = dObj.Object_Type == 'Port'

        result && portOrderAjustment(dObj, 1, direction)

        return result
      },
      /**
       * 批量设置连线样式
       * @param {*} vm
       * @param {*} edgeList 连线集合
       * @param {*} type 折线方式：0-直线 1-水平折线 2-垂直折线
       */
      setPolyline(graph, edges, type) {
        graph.getModel().beginUpdate()
        try {
          // 删除点
          edges.forEach((_edge) => {
            let geo = _edge.geometry.clone()
            // 设置折点
            // 从左到右排布
            if (type == 1) {
              let x = 0
              // x坐标是相同的，以更大的值为准，间隔居中
              if (_edge.source.geometry.x > _edge.target.geometry.x) {
                x = _edge.target.geometry.x + _edge.target.geometry.width
                x = x + (_edge.source.geometry.x - x) / 2
              } else {
                x = _edge.source.geometry.x + _edge.source.geometry.width
                x = x + (_edge.target.geometry.x - x) / 2
              }
              // y坐标是cell的y + 高度/2
              geo.points = [
                // 下标0是源折点
                new mxPoint(
                  x,
                  _edge.source.geometry.y + _edge.source.geometry.height / 2
                ),
                // 下标1是目标折点
                new mxPoint(
                  x,
                  _edge.target.geometry.y + _edge.target.geometry.height / 2
                ),
              ]
            }
            // 从上到下排布
            else if (type == 2) {
              let y = 0
              // y坐标是相同的，以更大的值为准
              if (_edge.source.geometry.y > _edge.target.geometry.y) {
                y = _edge.source.geometry.y - _edge.source.geometry.height / 2
              } else {
                y = _edge.target.geometry.y - _edge.target.geometry.height / 2
              }
              geo.points = [
                // 下标0是源折点
                new mxPoint(
                  _edge.source.geometry.x + _edge.source.geometry.width / 2,
                  y
                ),
                // 下标1是目标折点
                new mxPoint(
                  _edge.target.geometry.x + _edge.target.geometry.width / 2,
                  y
                ),
              ]
            } else {
              geo.points = null
            }
            graph.getModel().setGeometry(_edge, geo)
          })
          // 设置样式
          if (type == 0) {
            graph.setCellStyles('edgeStyle', null, edges)
            graph.setCellStyles('shape', null, edges)
          } else if (type == 1) {
            graph.setCellStyles('edgeStyle', 'orthogonalEdgeStyle', edges)
            graph.setCellStyles('elbow', null, edges)
            graph.setCellStyles('shape', null, edges)
          } else if (type == 2) {
            graph.setCellStyles('edgeStyle', 'orthogonalEdgeStyle', edges)
            graph.setCellStyles('elbow', 'vertical', edges)
            graph.setCellStyles('shape', null, edges)
          }
        } finally {
          graph.getModel().endUpdate()
        }
      },
    },
    mounted() {
      let vm = this
      // 监控容器尺寸变更，主动告知mxGraph更新布局
      // 触发此事件的场景包括：上下左右视图显隐导致diagramManager尺寸变更；图切换panel显隐；浏览器缩放变更；
      const dom = this.$refs.box.parentElement
      this.observer = new ResizeObserver(() => {
        let graph = vm.graph
        if (graph) {
          // graph视图重新验证
          graph.view.refresh()
          // 选中项恢复
          graph.selectionCellsHandler.refresh()

          //
        }
      })
      this.observer.observe(dom, { box: 'border-box' })
      // 检测浏览器兼容性
      if (!mxClient.isBrowserSupported()) {
        this.$message.error(
          '当前浏览器不支持拓扑图功能，请更换浏览器访问，建议使用Chrome浏览器访问!'
        )
      } else {
        // Overridden to define per-shape connection points
        mxGraph.prototype.getAllConnectionConstraints = function (terminal) {
          if (terminal != null && terminal.shape != null) {
            if (terminal.shape.stencil != null) {
              if (terminal.shape.stencil.constraints != null) {
                return terminal.shape.stencil.constraints
              }
            } else if (terminal.shape.constraints != null) {
              return terminal.shape.constraints
            }
          }
          return null
        }
        this.createGraph()
        this.eventCenter()
        this.configKeyEvent()
        this.configMouseEvent()
        this.configContextMenu()

        // 初始化完毕后，展示图
        this.ShowDiagram()
      }
    },
    destroyed() {
      this.observer.disconnect()
      this.keyHandler?.setEnabled(false)
      this.$set(this.diagram, 'unsave', false)
      this.autoSave = false
    },
  }
</script>
<style scoped>
  /*scale-box的样式设置*/
  .scale-box {
    position: absolute;
    bottom: 16px;
    right: 16px;
    padding: 5px;
    border: 1px solid #ccc;
    border-radius: 4px;
    background-color: rgba(255, 255, 255, 0.8);
    font-size: 12px;
  }
</style>
<style lang="scss" scoped>
  @import './index.scss';
</style>
