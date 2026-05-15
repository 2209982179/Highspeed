<template>
  <!-- 封装弹框 -->
  <div id="TabView" class="TabView">
    <div class="FaultTree">
      <el-button :loading="loading" @click="GetData">测试数据</el-button>
      <div ref="graph_container" class="graph-container"></div>
    </div>
    <div
      ref="customMenu"
      style="position: absolute; display: none; z-index: 1000"
    >
      <!-- 自定义菜单内容 -->
      <button @click="menuOption1">选项1</button>
      <button @click="menuOption2">选项2</button>
    </div>
  </div>
</template>
<script>
  //画布
  import { createGraph } from '@/utils/dgraph'
  import mx from '@/mxgraph/graph'
  const { mxEvent, mxPoint, mxConstants, mxEdgeStyle } = mx

  export default {
    name: 'TabView',
    data() {
      return {
        title: '画布绘图测试', // 窗体标题
        dialogPopVisible: false, // 窗体显示控制
        width: '1100px',
        loading: false,
        graphPublic: null, //全局画布
        AllCT: 0, //统计节点数
        TestDatas: [],
      }
    },
    created() {},
    computed: {
      currentSelectedDiagramObjects: {
        get() {
          let cells = this.graphPublic?.getSelectionCells() ?? []
          if (cells.length == 0) {
            return []
          }
          let items = []
          cells.forEach((element) => {
            if (element.vertex) {
              items.push(element)
            }
          })
          return items
        },
      },
    },
    mounted() {
      this.InitialGraph()
    },
    watch: {
      currentSelectedDiagramObjects: {
        handler(val) {
          this.$store.commit('SafetyAnalysisAllSelectedDiagramObjects', val)
        },
        deep: true,
      },
    },
    methods: {
      GetData() {
        let vm = this
        try {
          vm.TestDatas = [
            {
              ID: 0,
              Name: '测试节点1',
              Type: '数据',
              X: 50,
              Y: 100,
              Width: 200,
              Height: 100,
            },
            {
              ID: 1,
              Name: '测试节点2',
              Type: '指令',
              X: 450,
              Y: 100,
              Width: 200,
              Height: 100,
            },
            {
              ID: 2,
              Name: '测试节点3',
              Type: '数据',
              X: 850,
              Y: 100,
              Width: 200,
              Height: 100,
            },
            {
              ID: 3,
              Name: '测试节点4',
              Type: '指令',
              X: 1250,
              Y: 100,
              Width: 200,
              Height: 100,
            },
            {
              ID: 4,
              Name: '测试节点5',
              Type: '数据',
              X: 1650,
              Y: 100,
              Width: 200,
              Height: 100,
            },
          ]
          vm.InitialGraph()
          vm.UpdateGraph()
        } catch (error) {
          console.error(error)
          vm.$message.error('错误')
        }
      },
      onBeforeClose(done) {
        done()
      },
      //画布绘制
      InitialGraph() {
        let vm = this
        //重置画布
        if (this.graphPublic == null) {
          var graphApp = createGraph(this.$refs.graph_container)
          var graph = graphApp.editor.graph

          //设置启用,先允许改变形状内容，后面禁用部分功能。
          graph.setEnabled(true)
          graph.setConnectable(false) //禁止连线
          // 节点样式
          var style = graph.getStylesheet().getDefaultVertexStyle()
          style[mxConstants.STYLE_EDITABLE] = false //禁用编辑
          style[mxConstants.STYLE_ROTATABLE] = true //禁用旋转
          style[mxConstants.STYLE_MOVABLE] = false //禁用移动
          style[mxConstants.STYLE_RESIZABLE] = false //禁止改变大小
          style[mxConstants.STYLE_ROUNDED] = false //禁止圆角
          style[mxConstants.STYLE_BENDABLE] = false //禁止弯曲
          style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE //文字对齐方式
          style[mxConstants.STYLE_FILLCOLOR] = '#fff' //填充色
          style[mxConstants.STYLE_FONTSIZE] = 14 //文字大小
          style[mxConstants.STYLE_FONTCOLOR] = '#0d0d0c' //文字颜色
          style[mxConstants.STYLE_WHITE_SPACE] = 'wrap' //自动换行
          style[mxConstants.STYLE_STROKECOLOR] = '#000000' //边框
          //连线样式
          style = graph.getStylesheet().getDefaultEdgeStyle()
          style[mxConstants.STYLE_EDITABLE] = false //禁用编辑
          style[mxConstants.STYLE_ROTATABLE] = true //禁用旋转
          style[mxConstants.STYLE_MOVABLE] = true //禁用移动
          style[mxConstants.STYLE_RESIZABLE] = false //禁止改变大小
          style[mxConstants.STYLE_ROUNDED] = false //禁止圆角
          style[mxConstants.STYLE_BENDABLE] = false //禁止弯曲
          style[mxConstants.STYLE_EDGE] = mxEdgeStyle.TopToBottom
          style[mxConstants.STYLE_STROKECOLOR] = 'rgb(115, 121, 133)' //连接线颜色
          delete graph.getStylesheet().getDefaultEdgeStyle()['endArrow'] //去掉箭头

          this.$refs.graph_container.addEventListener(
            'contextmenu',
            function (evt) {
              evt.preventDefault() // 阻止默认的右键菜单显示
              // 自定义菜单逻辑，例如显示一个Vue组件作为菜单
              vm.showCustomContextMenu(evt.clientX, evt.clientY)
            }
          )

          // 自定义 tooltip
          graph.setTooltips(true)
          graph.getTooltipForCell = function (cell) {
            var data = cell.data || {}
            var tip = []
            for (const key in data) {
              if (Object.hasOwnProperty.call(data, key)) {
                const value = data[key]
                tip.push(`${key}: ${value}`)
              }
            }
            return tip.length ? tip.join('\n') : cell.value
          }

          //注册事件
          this.graphPublic = graph
        }

        // 节点样式
        style = this.graphPublic.getStylesheet().getDefaultVertexStyle()
        style[mxConstants.STYLE_FONTSIZE] = 14 //文字大小
        style[mxConstants.STYLE_FONTCOLOR] = '#0d0d0c' //文字颜色

        //清空画布
        if (this.graphPublic != null) {
          var cells = this.graphPublic.getChildCells()
          if (cells.length > 0) {
            this.graphPublic.removeCells(cells)
          }
        }
      },
      //画布绘制
      UpdateGraph() {
        let vm = this

        //清空画布
        if (this.graphPublic != null) {
          var cells = this.graphPublic.getChildCells()
          if (cells.length > 0) {
            this.graphPublic.removeCells(cells)
          }
        }
        //绘制数据
        let root = this.graphPublic.getDefaultParent()
        this.graphPublic.getModel().beginUpdate()
        try {
          //提取后台数据
          this.AllCT = 0
          if (this.TestDatas.length > 0) {
            vm.SetActiveDatas(this.graphPublic, root, this.TestDatas)
          }
        } finally {
          this.graphPublic.getModel().endUpdate()
          this.$message.info('当前节点数量：' + this.AllCT)
        }
      },
      //创建节点
      SetActiveDatas(graph, root, TestData) {
        let vm = this
        try {
          if (TestData.length > 0) {
            var PreShape
            TestData.forEach((item) => {
              //创建图形外框
              this.AllCT++
              let shape = this.InsertCustomShape(
                graph,
                root,
                item.ID,
                item.Name,
                item.Type,
                item
              )

              if (this.AllCT > 1) {
                let edge = graph.insertEdge(root, null, '', PreShape, shape)

                const points = [
                  new mxPoint(
                    TestData[this.AllCT - 2].X + TestData[this.AllCT - 2].Width,
                    TestData[this.AllCT - 2].Y +
                      TestData[this.AllCT - 2].Height / 2
                  ),
                  new mxPoint(item.X, item.Y + item.Height / 2),
                ]

                let geo = graph.getCellGeometry(edge)
                if (geo != null) {
                  geo.points = points
                }
              }
              PreShape = shape
            })
          }
        } catch (error) {
          console.error(error)
          vm.$message.error('错误')
        }
      },
      //构建自定义图形
      InsertCustomShape(graph, root, id, name, type, item) {
        var showData = {
          Id: id,
          名称: name,
          类型: type,
          位置信息: '',
        } // 自定义数据
        //创建图形外框
        let Group0 = graph.insertVertex(
          root,
          null,
          '',
          item.X,
          item.Y,
          item.Width,
          item.Height,
          'rounded=0;whiteSpace=wrap;html=1;fillColor=#add8e6;strokeColor=#0d0d0c;strokeColor=1;'
        )

        var ColorSetting1 =
          'fillColor=#add8e6;strokeColor=#0d0d0c;strokeWidth=1;'

        Group0.data = showData
        //内部文本框1
        let tip_text = graph.insertVertex(
          Group0,
          null,
          item.ID + '\r\n' + item.Name,
          0,
          0,
          item.Width,
          item.Height / 2 - 10,
          'rounded=0;whiteSpace=wrap;html=1;' + ColorSetting1
        )
        tip_text.data = showData // 自定义数据

        //内部逻辑图形
        var shapesvg =
          'shape=ellipse;whiteSpace=wrap;html=1;rotation=-90;' + ColorSetting1

        //核心图形
        let shape = graph.insertVertex(
          Group0,
          null,
          '',
          item.Width / 2 - 25,
          item.Height / 2,
          50,
          50,
          shapesvg
        )
        shape.data = showData // 自定义数据
        graph.insertEdge(Group0, null, '', tip_text, shape)

        //返回整个外框
        return Group0
      },
      showCustomContextMenu(x, y) {
        // 这里可以创建一个Vue组件来作为右键菜单，例如使用Element UI的Menu组件或者自己定义的组件
        this.$refs.customMenu.style.left = x + 'px'
        this.$refs.customMenu.style.top = y + 'px'
        this.$refs.customMenu.style.display = 'block' // 显示自定义菜单
      },
      menuOption1() {
        this.$message.error('选项1被点击')
      },
      menuOption2() {
        this.$message.error('选项2被点击')
      },
    },
  }
</script>
<style lang="scss" scoped>
  .TabView {
    width: 99vw;
    height: 99vh;
  }

  .FaultTree {
    width: calc(98% - 25px) !important; /* 继承容器高度 */
    height: calc(98% - 25px) !important; /* 继承容器高度 */
    border: 1px solid rgba(255, 255, 255, 0.247);
  }

  .graph-container {
    width: 100% !important;
    height: 100% !important;
    line-height: 98%;
    position: absolute;
    overflow: auto;
    background-color: #fff !important;
  }
</style>
