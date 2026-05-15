<template>
  <!-- 封装弹框 -->
  <div class="TestCasePage">
    <div class="GroupViews">
      <div class="groupTestCaseListView1">
        <el-divider content-position="left">测试用例列表</el-divider>
        <div slot="footer" class="dialog-top1" style="text-align: left">
          <el-button :loading="loading" type="primary" @click="GetData">
            刷新
          </el-button>
          <el-button :loading="loading" type="primary" @click="TestCase_AddNew">
            新增
          </el-button>
          <el-button :loading="loading" type="primary" @click="TestCase_Edit">
            编辑
          </el-button>
          <el-button :loading="loading" type="primary" @click="TestCase_Delete">
            删除
          </el-button>
          <el-button :loading="loading" type="primary" @click="TestCase_Execute">执行</el-button>
        </div>
        <div style="overflow: auto">
          <el-tree
            :data="Treedata"
            node-key="TestId"
            :props="defaultProps"
            default-expand-all
            @current-change="handleCurrentChange"
          ></el-tree>
        </div>
      </div>
      <div class="groupTestCaseListView2">
        <div>
          <div>
            <el-divider content-position="left">测试步骤</el-divider>
            <div
              slot="footer"
              class="dialog-top2"
              style="
                text-align: left;
                display: flex;
                justify-content: space-between;
                align-items: center;
                width: 100%;
              "
            >
              <div>
                <el-button
                  :loading="loading"
                  type="primary"
                  @click="TrainSetting"
                >
                  关联列车属性
                </el-button>
                <el-button :loading="loading" type="primary" @click="Savesign">
                  记录信号
                </el-button>
                <el-button :loading="loading" type="primary" @click="InitSign">
                  用例前置条件
                </el-button>
                <!-- 新建步骤下拉菜单 -->
                 <el-button :loading="loading" type="primary" @click="StepRunTest('????')">
                    新建步骤<i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>

                <el-button
                  :loading="loading"
                  type="primary"
                  @click="TestStep_Edit"
                >
                  编辑步骤
                </el-button>
                <el-button
                  :loading="loading"
                  type="primary"
                  @click="TestStep_Delete"
                >
                  删除步骤
                </el-button>
              </div>
              <div style="padding-right: 20px">
                <el-link type="primary" @click="backtologin">返回登录</el-link>
              </div>
            </div>
            <div class="DetailsView">
            <!-- 步骤列表（支持 IF/FOR 树形展示） -->
              <div v-if="isTreeView" :key="treeRenderKey" class="step-tree-view">
                <TestStepTreeNode
                  v-for="step in TestCommands"
                  :key="step.TestStepID"
                  :step="step"
                  :current-test-step="CurrentTestStep"
                  :expanded-map="expandedSteps"
                  :default-image-src="defaultImageSrc"
                  :depth="0"
                  @row-click="handleRowClick"
                  @drag-over="onStepDragOver"
                  @drag-leave="onStepDragLeave"
                  @drop="onStepDrop"
                  @drag-start="onStepDragStart"
                  @drag-end="onStepDragEnd"
                  @toggle-expand="toggleExpand"
                  @add-child="addChildStep"
                />
                <div v-if="TestCommands.length === 0" class="empty-state">
                  <div style="font-size: 32px; color: #ccc; margin-bottom: 8px">📂</div>
                  <div style="font-size: 13px; color: #bbb">请从左侧选择测试用例</div>
                  <div style="font-size: 11px; color: #ddd; margin-top: 4px">支持 IF判断 · FOR循环 · 多层子节点</div>
                </div>
              </div>

              <!-- 原始 el-table 表格视图（回退兼容） -->
              <el-table
                v-else
                class="TestCommands"
                ref="multipleTableTestCase"
                highlight-current-row
                :data="TestCommands"
                row-key="TestStepID"
                border
                v-loading="false"
                tooltip-effect="dark"
                default-expand-all
                @row-click="handleRowClick"
                :tree-props="{
                  children: 'children',
                  hasChildren: 'hasChildren',
                }"
              >
                <el-table-column label="序号" width="100" prop="TestCaseID">
                  <template slot-scope="scope">{{
                    scope.row.DisplayOrder || scope.row.TestCaseID
                  }}</template>
                </el-table-column>
                <el-table-column
                  label="步骤名称"
                  width="100"
                  prop="TestStepName"
                >
                  <template slot-scope="scope">
                    {{ scope.row.TestStepName }}
                  </template>
                </el-table-column>
                <el-table-column
                  label="测试类型"
                  width="180"
                  prop="sendReceiveType"
                >
                  <template slot-scope="scope">
                    <el-select
                      v-model="scope.row.sendReceiveType"
                      clearable
                      :disabled="true"
                      style="width: auto; height: auto"
                    >
                      <el-option
                        v-for="(Enum, index) in StepsendReceiveType"
                        :key="'StepsendReceiveType_' + index"
                        :label="Enum.text"
                        :value="Enum.value"
                      ></el-option>
                    </el-select>
                  </template>
                </el-table-column>
                <el-table-column label="步骤值" width="100" prop="StepValue">
                  <template slot-scope="scope">{{
                    scope.row.StepValue
                  }}</template>
                </el-table-column>
                <el-table-column label="备注" width="300" prop="Remark">
                  <template slot-scope="scope">{{ scope.row.Remark }}</template>
                </el-table-column>
                <el-table-column label="执行" width="80" prop="Execute">
                  <template slot-scope="scope">
                    <el-image
                      style="width: 16px; height: 16px"
                      :src="scope.row.url"
                      @error="handleImageError"
                    ></el-image>
                  </template>
                </el-table-column>
                <el-table-column label="打印log" width="80" prop="Execute">
                  <template slot-scope="scope">
                    <el-image
                      style="width: 16px; height: 16px"
                      :src="scope.row.Logurl"
                      @error="handleImageError"
                    ></el-image>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </div>
        </div>

        <div>
          <el-divider content-position="left">测试用例日志输出</el-divider>
          <div
            slot="footer"
            class="dialog-top2"
            style="
              text-align: left;
              display: flex;
              justify-content: space-between;
              align-items: center;
            "
          ></div>
          <el-input
            type="textarea"
            :rows="15"
            v-model="textarea"
            :disabled="true"
          ></el-input>
        </div>
      </div>
      <div class="groupTestCaseListView3">
        <div class="ChartView">
          <div
            v-if="showecharts1"
            ref="echarts1"
            style="
              margin: 0px;
              padding: 0px;
              width: 320px;
              height: 250px;
            "
          ></div>
          <div
            v-if="showecharts2"
            ref="echarts2"
            style="
              margin: 0px;
              padding: 0px;
              width: 320px;
              height: 250px;
            "
          ></div>
          <div
            v-if="showecharts3"
            ref="echarts3"
            style="
              margin: 0px;
              padding: 0px;
              width: 320px;
              height: 250px;
            "
          ></div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import {
  GetTestCaseList,
  GetTestStepDatas,
  DeleteTestCase,
  DeleteTestStep,
  GetTestRecodeDatas,
  ExecuteTestCase,
  UpdateTestStep,
} from '@/API/Admin'
import TestCaseEditDialog from '@/views/business/TestCaseList/TestCaseEdit.vue'
import TestStepAddExecuteDialog from '@/views/business/TestCaseList/TestStepAddExecute.vue'
import TestStepRecodeSignDialog from '@/views/business/TestCaseList/TestStepRecodeSign.vue'
import TestStepTreeNode from '@/views/business/TestCaseList/components/TestStepTreeNode.vue'
import TrainSettingImforDialog from '@/views/business/TestCaseList/TrainSettingImfor.vue'
import TestCaseInitSettingDialog from '@/views/business/TestCaseList/TestCaseInitSetting.vue'
import router from '@/router'
export default {
  name: 'DialogPopup',
  components: {
    TestStepTreeNode,
  },
  data() {
    return {
      title: '测试用例列表',
      width: '1000px',
      parent: {},
      loading: false,
      TestCaseList: [],
      TestCommands: [],
      TrainSettingDatas: [],
      TestRecodesDatas: [],
      NewTestRecodesDatas: [],
      TestRecodesOptions: [],
      CurrentTestStep: [],
      ActTestCase: {},
      TestId: '',
      StepsendReceiveType: [
        { value: 0, text: '发送信号' },
        { value: 1, text: '接收信号' },
        { value: 2, text: '其他信号' },
      ],
      defaultProps: {
        children: 'child', //子类的名称
        label: 'TestName', //接口返回文字的字段
      },
      TestCaseMixClassdata: [],
      Treedata: [],
      testCaseModel: [],
      url: require('@/image/TestCase_SendSignal_16x16.png'),
      defaultImageSrc: require('@/image/TestCase_SendSignal_16x16.png'),
      showecharts1: true,
      showecharts2: true,
      showecharts3: true,
      currentNode: null,
      // 视图切换
      isTreeView: true,
      // 展开/折叠状态
      expandedSteps: {},
      // 拖拽状态
      dropTargetRow: null,
      dropPosition: '', // 'before' | 'after' | 'child'
      dragSaving: false,
      treeRenderKey: 0,
    }
  },
  created() {
    this.GetData()
  },
  mounted() {
    this.ShowCharts()
  },
  methods: {
    handleImageError(event) {
      event.target.src = this.defaultImageSrc
    },
    normalizeStepType(stepType) {
      if (stepType === 1 || stepType === 'Execute') return 'Execute'
      if (stepType === 3 || stepType === 'If' || stepType === 'IF判断') return 'IF判断'
      if (stepType === 4 || stepType === 'For' || stepType === 'FOR循环') return 'FOR循环'
      if (stepType === 0 || stepType === 'Recode') return 'Recode'
      if (stepType === 2 || stepType === 'Temporality') return 'Temporality'
      return stepType || 'Execute'
    },
    normalizeSendReceiveType(sendReceiveType) {
      if (sendReceiveType === 0 || sendReceiveType === 'Send') return 0
      if (sendReceiveType === 1 || sendReceiveType === 'Receive') return 1
      return 2
    },
    normalizeStepTypeForApi(stepType) {
      if (stepType === 1 || stepType === 'Execute') return 'Execute'
      if (stepType === 3 || stepType === 'If' || stepType === 'IF判断') return 'If'
      if (stepType === 4 || stepType === 'For' || stepType === 'FOR循环') return 'For'
      if (stepType === 0 || stepType === 'Recode') return 'Recode'
      if (stepType === 2 || stepType === 'Temporality') return 'Temporality'
      return stepType || 'Execute'
    },
    normalizeSendReceiveTypeForApi(sendReceiveType) {
      if (sendReceiveType === 0 || sendReceiveType === 'Send') return 'Send'
      if (sendReceiveType === 1 || sendReceiveType === 'Receive') return 'Receive'
      return 'Other'
    },
    isIfStep(step) {
      return this.normalizeStepType(step.TestStepType) === 'IF判断'
    },
    isForStep(step) {
      return this.normalizeStepType(step.TestStepType) === 'FOR循环'
    },
    normalizeTestSteps(stepList) {
      const list = (stepList || []).map(item => ({
        ...item,
        TestStepType: this.normalizeStepType(item.TestStepType),
        sendReceiveType: this.normalizeSendReceiveType(item.sendReceiveType),
        BranchType: item.BranchType || item.ExtendedField1 || '',
        BindTestStepID: item.BindTestStepID || (item.ExtendedField2 ? Number(item.ExtendedField2) : null),
        IfOperator: item.IfOperator || item.ExtendedField3 || '',
        IfExpectedValue: item.IfExpectedValue || item.ExtendedField4 || '',
        IfCondition: item.IfCondition || ((item.ExtendedField3 && item.ExtendedField4) ? `${item.ExtendedField3} ${item.ExtendedField4}` : ''),
        LoopCount: Number(item.LoopCount || item.ExtendedField5 || 1),
        ForVar: item.ForVar || 'i',
        ForStart: 1,
        ForEnd: Number(item.LoopCount || item.ExtendedField5 || 1),
        ForStep: 1,
        children: [],
      }))

      const map = {}
      list.forEach(step => {
        map[step.TestStepID] = step
      })

      const roots = []
      list.forEach(step => {
        if (step.ParentID && map[step.ParentID]) {
          map[step.ParentID].children.push(step)
        } else {
          roots.push(step)
        }
      })

      const sortChildren = (steps) => {
        steps.sort((a, b) => {
          const prevA = Number(a.PreviousID || -1)
          const prevB = Number(b.PreviousID || -1)
          if (prevA !== prevB) return prevA - prevB
          return Number(a.TestStepID) - Number(b.TestStepID)
        })
        steps.forEach(step => sortChildren(step.children || []))
      }

      sortChildren(roots)
      this.assignDisplayOrder(roots)
      return roots
    },
    flattenSteps(stepList) {
      const result = []
      const walk = (steps) => {
        (steps || []).forEach(step => {
          result.push(step)
          if (step.children && step.children.length > 0) {
            walk(step.children)
          }
        })
      }
      walk(stepList || [])
      return result
    },
    assignDisplayOrder(stepList) {
      let order = 1
      const walk = (steps) => {
        (steps || []).forEach(step => {
          this.$set(step, 'DisplayOrder', order++)
          if (step.children && step.children.length > 0) {
            walk(step.children)
          }
        })
      }
      walk(stepList || [])
    },
    refreshStepTree() {
      this.treeRenderKey += 1
    },

    // ═══════════════════════════════════════════════
    // IF/FOR 树形视图辅助方法
    // ═══════════════════════════════════════════════

    /** 获取步骤类型显示标签 */
    getStepTypeLabel(step) {
      const map = {
        'IF判断': 'IF',
        'FOR循环': 'FOR',
        'Execute': '发送',
        'Receive': '接收',
        'Custom': '自定义',
      }
      if (step.sendReceiveType === 0) return '发送'
      if (step.sendReceiveType === 1) return '接收'
      return map[step.TestStepType] || step.TestStepType || '自定义'
    },

    /** 閼惧嘲褰囧銉╊€冨鐣岀彿閺嶅嘲绱?*/
    getStepBadgeClass(step) {
      if (step.TestStepType === 'IF判断') return 'badge-if'
      if (step.TestStepType === 'FOR循环') return 'badge-for'
      return 'badge-normal'
    },

   /** 获取 FOR 循环描述文本 */
    getForDesc(step) {
      if (step.ForMode === 'list') {
        return `${step.ForVar} in [${step.ForList || ''}]`
      }
      return `${step.ForVar} = ${step.ForStart || 0} ~ ${step.ForEnd || ''}, step=${step.ForStep || 1}`
    },

    /** 判断步骤是否有分支子节点 */
    hasBranchChildren(step) {
      if (step.TestStepType === 'IF判断') {
        const thenChildren = step.children ? step.children.filter(c => c.BranchType === 'then') : []
        const elseChildren = step.children ? step.children.filter(c => c.BranchType === 'else') : []
        return thenChildren.length > 0 || elseChildren.length > 0
      }
      if (step.TestStepType === 'FOR循环') {
        const bodyChildren = step.children ? step.children.filter(c => c.BranchType === 'body') : []
        return bodyChildren.length > 0
      }
      return step.children && step.children.length > 0
    },

    /** 获取指定分支的子步骤 */
    getBranchChildren(step, branchType) {
      if (!step.children) return []
      return step.children.filter(c => c.BranchType === branchType)
    },

   /** 展开/折叠状态 */
    toggleExpand(stepId) {
      this.$set(this.expandedSteps, stepId, !this.isExpanded(stepId))
    },
    isExpanded(stepId) {
      return this.expandedSteps[stepId] !== false // 默认展开
    },

    /** 向控制流步骤添加子步骤 */
    addChildStep(parentStepId, stepType, branchType) {
      if (this.currentNode == null) {
        this.$message.error('请选择测试用例节点！')
        return
      }
      try {
        window.$showDialog(TestStepAddExecuteDialog, {
          TestCaseID: this.currentNode.TestId,
          PreviousID: -1,
          ParentID: parentStepId,
          TestStepID: -1,
          TestStepType: stepType,
          BranchType: branchType,
          AvailableBasicSteps: this.flattenSteps(this.TestCommands),
          TestStepName: '',
          StepValue: '',
          Remark: '',

          selected: async (checkC) => {
            if (checkC) {
              GetTestStepDatas({ TestCaseID: this.currentNode.TestId }).then(
                (res) => {
                  this.TestCommands = this.normalizeTestSteps(res)
                }
              )
            }
          },
        })
      } catch (error) {
        console.error(error)
        this.$message.error('添加子步骤失败')
      }
    },

    // ═══════════════════════════════════════════════
    // 拖拽排序方法
    // ═══════════════════════════════════════════════

    /** 拖拽开始 */
    onStepDragStart(e, step) {
      if (this.dragSaving) {
        e.preventDefault()
        return
      }
      this.dragRow = step
      e.dataTransfer.effectAllowed = 'move'
      e.dataTransfer.setData('text/plain', String(step.TestStepID))
      this.$nextTick(() => {
        const node = e.target.closest('.step-tree-node')
        if (node) node.classList.add('dragging')
      })
      console.log('drag start')
    },

    /** 拖拽结束 */
    onStepDragEnd() {
      this.dragRow = null
      this.dropTargetRow = null
      this.dropPosition = ''
      document.querySelectorAll('.step-tree-node').forEach(n =>
        n.classList.remove('dragging', 'drag-over-top', 'drag-over-bottom', 'drag-over-child')
      )
      
      console.log('drag end')
    },

    /** 拖拽经过目标节点 — 计算上/中/下位置 */
    onStepDragOver(e, targetStep) {
      if (!this.dragRow || this.dragRow.TestStepID === targetStep.TestStepID) return
      if (this.isDescendant(this.dragRow, targetStep.TestStepID)) return
      e.preventDefault()
      e.dataTransfer.dropEffect = 'move'

      const node = e.currentTarget
      const rect = node.getBoundingClientRect()
      const ratio = (e.clientY - rect.top) / rect.height
      const pos = ratio < 0.28 ? 'before' : ratio > 0.72 ? 'after' : 'child'

      node.classList.remove('drag-over-top', 'drag-over-bottom', 'drag-over-child')
      if (pos === 'before') node.classList.add('drag-over-top')
      else if (pos === 'after') node.classList.add('drag-over-bottom')
      else node.classList.add('drag-over-child')

      this.dropTargetRow = targetStep
      this.dropPosition = pos
    },

    /** 拖拽离开目标节点 */
    onStepDragLeave(e) {
      const node = e.currentTarget
      if (!node.contains(e.relatedTarget)) {
        node.classList.remove('drag-over-top', 'drag-over-bottom', 'drag-over-child')
      }
    },

    /** 拖拽离开目标节点 */
    async onStepDrop(e, targetStep) {
      e.preventDefault()
      e.stopPropagation()
      const node = e.currentTarget
      node.classList.remove('drag-over-top', 'drag-over-bottom', 'drag-over-child')

      if (!this.dragRow || !this.dropTargetRow) return
      if (this.dragRow.TestStepID === targetStep.TestStepID) return

      try {
        await this.handleDropRow(this.dragRow, this.dropTargetRow, this.dropPosition)
      } finally {
        this.onStepDragEnd()
      }
    },

    /** 拖拽离开目标节点 */
    isDescendant(step, targetId) {
      if (!step.children || step.children.length === 0) return false
      for (const child of step.children) {
        if (child.TestStepID === targetId) return true
        if (this.isDescendant(child, targetId)) return true
      }
      return false
    },

    /** 处理拖拽落点 — 调整 TestCommands 顺序 */
    async handleDropRow(srcStep, tgtStep, position) {
      const previousTree = JSON.parse(JSON.stringify(this.TestCommands))

      const removed = this.removeStepFromTree(this.TestCommands, srcStep.TestStepID)
      if (!removed) return

      let inserted = false
      if (position === 'before') {
        inserted = this.insertStepBefore(this.TestCommands, tgtStep, removed)
      } else if (position === 'after') {
        inserted = this.insertStepAfter(this.TestCommands, tgtStep, removed)
      } else if (position === 'child') {
        this.insertStepAsChild(tgtStep, removed)
        inserted = true
      }

      if (!inserted) {
        this.TestCommands = previousTree
        this.refreshStepTree()
        throw new Error('Failed to insert dragged step into tree')
      }

      this.rebuildStepRelations(this.TestCommands)
      this.TestCommands = [...this.TestCommands]
      this.assignDisplayOrder(this.TestCommands)
      this.refreshStepTree()

      try {
        await this.saveDraggedStepOrder()
        await this.reloadCurrentTestSteps()
        this.$message.success('拖拽顺序已保存并刷新')
      } catch (error) {
        console.error(error)
        this.TestCommands = previousTree
        this.assignDisplayOrder(this.TestCommands)
        this.refreshStepTree()
        this.$message.error(`拖拽保存失败：${error && error.message ? error.message : '未知错误'}`)
        await this.reloadCurrentTestSteps()
      }
    },

    /** 从树中移除指定步骤 */
    removeStepFromTree(list, stepId) {
      for (let i = 0; i < list.length; i++) {
        if (list[i].TestStepID === stepId) {
          return list.splice(i, 1)[0]
        }
        if (list[i].children && list[i].children.length > 0) {
          const found = this.removeStepFromTree(list[i].children, stepId)
          if (found) return found
        }
      }
      return null
    },

   /** 从树中移除指定步骤 */
    insertStepBefore(list, targetStep, step) {
      for (let i = 0; i < list.length; i++) {
        if (list[i].TestStepID === targetStep.TestStepID) {
          step.ParentID = targetStep.ParentID || 0
          step.BranchType = targetStep.BranchType || ''
          list.splice(i, 0, step)
          return true
        }
        if (list[i].children && list[i].children.length > 0) {
          if (this.insertStepBefore(list[i].children, targetStep, step)) return true
        }
      }
      return false
    },

    /** 在目标步骤后插入 */
    insertStepAfter(list, targetStep, step) {
      for (let i = 0; i < list.length; i++) {
        if (list[i].TestStepID === targetStep.TestStepID) {
          step.ParentID = targetStep.ParentID || 0
          step.BranchType = targetStep.BranchType || ''
          list.splice(i + 1, 0, step)
          return true
        }
        if (list[i].children && list[i].children.length > 0) {
          if (this.insertStepAfter(list[i].children, targetStep, step)) return true
        }
      }
      return false
    },

    /** 在父步骤下插入子步骤 */
    insertStepAsChild(parentStep, step) {
      if (!parentStep.children) {
        this.$set(parentStep, 'children', [])
      }
      step.BranchType = this.getDefaultChildBranchType(parentStep)
      step.ParentID = parentStep.TestStepID
      parentStep.children.push(step)
    },
    getDefaultChildBranchType(parentStep) {
      if (this.isIfStep(parentStep)) return 'then'
      if (this.isForStep(parentStep)) return 'body'
      return ''
    },
    rebuildStepRelations(stepList, parentId = 0, branchType = '') {
      (stepList || []).forEach((step, index) => {
        step.ParentID = Number(parentId || 0)
        step.BranchType = branchType || ''
        step.PreviousID = index === 0 ? -1 : stepList[index - 1].TestStepID

        if (!step.children || step.children.length === 0) {
          return
        }

        if (this.isIfStep(step)) {
          const thenChildren = step.children.filter(child => (child.BranchType || '') === 'then')
          const elseChildren = step.children.filter(child => (child.BranchType || '') === 'else')
          step.children = [...thenChildren, ...elseChildren]
          this.rebuildStepRelations(thenChildren, step.TestStepID, 'then')
          this.rebuildStepRelations(elseChildren, step.TestStepID, 'else')
          return
        }

        if (this.isForStep(step)) {
          const bodyChildren = step.children.filter(child => (child.BranchType || '') === 'body')
          step.children = [...bodyChildren]
          this.rebuildStepRelations(bodyChildren, step.TestStepID, 'body')
          return
        }

        this.rebuildStepRelations(step.children, step.TestStepID, '')
      })
    },
    buildStepUpdatePayload(step) {
      return {
        TestCaseID: this.currentNode.TestId,
        TestStepID: step.TestStepID,
        PreviousID: Number(step.PreviousID || -1),
        ParentID: Number(step.ParentID || 0),
        BranchType: step.BranchType || '',
        TestStepType: this.normalizeStepTypeForApi(step.TestStepType),
        TestStepName: step.TestStepName || '',
        StepValue: step.StepValue || '',
        sendReceiveType: this.normalizeSendReceiveTypeForApi(step.sendReceiveType),
        Remark: step.Remark || '',
        BindTestStepID: step.BindTestStepID || null,
        IfOperator: step.IfOperator || null,
        IfExpectedValue: step.IfExpectedValue || '',
        LoopCount: step.LoopCount || 1,
      }
    },
    async saveDraggedStepOrder() {
      if (!this.currentNode || !this.currentNode.TestId) return
      this.dragSaving = true
      this.loading = true
      this.$showLoading()

      try {
        const steps = this.flattenSteps(this.TestCommands)
        for (const step of steps) {
          const result = await UpdateTestStep(this.buildStepUpdatePayload(step))
          if (result !== true) {
            throw new Error(`UpdateTestStep failed: ${step.TestStepID}`)
          }
        }
      } finally {
        this.dragSaving = false
        this.loading = false
        this.$hideLoading()
      }
    },
    async reloadCurrentTestSteps() {
      if (!this.currentNode || !this.currentNode.TestId) return
      const res = await GetTestStepDatas({ TestCaseID: this.currentNode.TestId })
      this.TestCommands = this.normalizeTestSteps(res)
      this.refreshStepTree()
    },
    ShowCharts() {
      const chartOptions = {
        grid: {
          top: '10%',
          left: '3%',
          right: '10%',
          bottom: '10%',
          containLabel: true,
        },
        title: {
          text: '速度',
          left: 'center',
          textStyle: {
            fontSize: 15,
            color: '#333',
          },
        },
        xAxis: {
          type: 'category',
          data: [1, 2, 3, 4, 5],
          axisLabel: {
            rotate: 0,
          },
        },
        yAxis: { type: 'value' },
        tooltip: {
          trigger: 'axis',
        },
        series: [
          {
            data: [1, 2, 3, 4, 5],
            type: 'line',
            showBackground: true,
            backgroundStyle: {
              color: '#33669966',
            },
          },
        ],
      }

      const chartOptions1 = {
        grid: {
          top: '10%',
          left: '3%',
          right: '10%',
          bottom: '10%',
          containLabel: true,
        },
        title: {
          text: '牵引力',
          left: 'center',
          textStyle: {
            fontSize: 15,
            color: '#333',
          },
        },
        xAxis: {
          type: 'category',
          data: [1, 2, 3, 4, 5],
          axisLabel: {
            rotate: 0,
          },
        },
        yAxis: { type: 'value' },
        tooltip: {
          trigger: 'axis',
        },
        series: [
          {
            data: [1, 2, 3, 4, 5],
            type: 'line',
            showBackground: true,
            backgroundStyle: {
              color: '#33669966',
            },
          },
        ],
      }

      const chartOptions2 = {
        grid: {
          top: '10%',
          left: '3%',
          right: '10%',
          bottom: '10%',
          containLabel: true,
        },
        title: {
          text: '制动',
          left: 'center',
          textStyle: {
            fontSize: 15,
            color: '#333',
          },
        },
        xAxis: {
          type: 'category',
          data: [1, 2, 3, 4, 5],
          axisLabel: {
            rotate: 0,
          },
        },
        yAxis: { type: 'value' },
        tooltip: {
          trigger: 'axis',
        },
        series: [
          {
            data: [1, 2, 3, 4, 5],
            type: 'line',
            showBackground: true,
            backgroundStyle: {
              color: '#33669966',
            },
          },
        ],
      }

      var mychart1 = this.$echarts.init(this.$refs.echarts1)
      mychart1.setOption(chartOptions)

      var mychart2 = this.$echarts.init(this.$refs.echarts2)
      mychart2.setOption(chartOptions1)

      var mychart3 = this.$echarts.init(this.$refs.echarts3)
      mychart3.setOption(chartOptions2)
    },
    GetData() {
      let vm = this
      try {
        GetTestCaseList().then((res) => {
          vm.TestCaseMixClassdata = res
          vm.Treedata = res.testCaseTree
          console.log('vm.Treedata', vm.Treedata)
          vm.testCaseModel = res.testCaseModel
          console.log('vm.testCaseModel', vm.testCaseModel)
        })
      } catch (error) {
        console.error(error)
        this.$message.error('闁挎错误')
      }

      this.ActTestCase == null
      this.TestCommands = []
    },
    handleCurrentChange(data) {
      this.currentNode = data
      this.TrainSettingDatas = this.testCaseModel.find(
        (x) => x.TestCaseID == this.currentNode.TestId
      ).TestTrainInformation
      this.TestRecodesDatas = this.testCaseModel.find(
        (x) => x.TestCaseID == this.currentNode.TestId
      ).TestRecodes
      console.log('this.currentNode', this.currentNode)
      console.log('this.TrainSettingDatas', this.TrainSettingDatas)
      console.log('this.TestRecodesDatas', this.TestRecodesDatas)
      try {
        GetTestStepDatas({ TestCaseID: this.currentNode.TestId }).then(
          (res) => {
            this.TestCommands = this.normalizeTestSteps(res)
          }
        )
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    handleRowClick(row) {
      try {
        this.CurrentTestStep = row
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    TrainSetting() {
      let vm = this
      var TestCaseID = -1
      console.log("TrainName", vm.TrainSettingDatas.TrainName)
      console.log("vm.TrainSettingDatas", vm.TrainSettingDatas)
      try {
        window.$showDialog(TrainSettingImforDialog, {
          TestCaseID: TestCaseID,
          TrainName: vm.TrainSettingDatas.TrainName,
          TrainSettingDatas: vm.TrainSettingDatas,
          FormationType: vm.TrainSettingDatas.FormationType,
          MaximumOperatingSpeed: vm.TrainSettingDatas.MaximumOperatingSpeed,
          AverageTravelSpeed: vm.TrainSettingDatas.AverageTravelSpeed,
          AverageDwellTime: vm.TrainSettingDatas.AverageDwellTime,
          AverageAcceleration: vm.TrainSettingDatas.AverageAcceleration,
          AverageDeceleration: vm.TrainSettingDatas.AverageDeceleration,
          TrainImpactRate: vm.TrainSettingDatas.TrainImpactRate,
          RequirementsFaultStatus: vm.TrainSettingDatas.RequirementsFaultStatus,
          RequirementsRampRescue: vm.TrainSettingDatas.RequirementsRampRescue,
          RequirementsNoiseLevelIn:
            vm.TrainSettingDatas.RequirementsNoiseLevelIn,
          RequirementsNoiseLevelOut:
            vm.TrainSettingDatas.RequirementsNoiseLevelOut,
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },

    /** 新建步骤（支持所有类型：Execute / Temporality / IF判断 / FOR循环） */
    StepRunTest() {
      let vm = this
      if (this.currentNode == null) {
        this.$message.error('请选择测试用例')
        return
      }
      var TestCaseID = this.currentNode.TestId
      var TestStepID = -1
      var Remark = ''

      try {
          window.$showDialog(TestStepAddExecuteDialog, {
            TestCaseID: TestCaseID,
            PreviousID: vm.CurrentTestStep ? vm.CurrentTestStep.TestStepID : -1,
            ParentID: '0',
            TestStepID: TestStepID,
            TestStepType: 'Execute',
            BranchType: null,
            AvailableBasicSteps: this.flattenSteps(this.TestCommands),
            TestStepName: '',
            StepValue: '',
            Remark: Remark,

            selected: async (checkC) => {
              if (checkC) {
                GetTestStepDatas({ TestCaseID: this.currentNode.TestId }).then(
                  (res) => {
                    this.TestCommands = this.normalizeTestSteps(res)
                  }
                )
              }
            },
          })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },

    /** 编辑选中步骤 */
    TestStep_Edit() {
      let vm = this
      if (this.CurrentTestStep == null || !this.CurrentTestStep.TestStepID) {
        this.$message.error('请选择测试步骤')
        return
      }
      try {
        const step = this.CurrentTestStep
        window.$showDialog(TestStepAddExecuteDialog, {
          TestCaseID: vm.currentNode.TestId,
          TestStepID: step.TestStepID,
          PreviousID: step.PreviousID || -1,
          ParentID: step.ParentID || '0',
          BranchType: step.BranchType || null,
          TestStepType: step.TestStepType || 'Execute',
          AvailableBasicSteps: this.flattenSteps(this.TestCommands),
          TestStepName: step.TestStepName || '',
          StepValue: step.StepValue || '',
          Remark: step.Remark || '',
          BindTestStepID: step.BindTestStepID || null,
          IfOperator: step.IfOperator || '>',
          IfExpectedValue: step.IfExpectedValue || '',
          LoopCount: step.LoopCount || 1,

          selected: async (checkC) => {
            if (checkC) {
              GetTestStepDatas({ TestCaseID: this.currentNode.TestId }).then(
                (res) => {
                  this.TestCommands = this.normalizeTestSteps(res)
                }
              )
            }
          },
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },

    InitSign() {
      let vm = this

      try {
        window.$showDialog(TestCaseInitSettingDialog, {
          TestCaseID: vm.currentNode.TestId,
          ExtendedField7: this.testCaseModel.find(
            (x) => x.TestCaseID == vm.currentNode.TestId
          ).ExtendedField7,
          ExtendedField8: this.testCaseModel.find(
            (x) => x.TestCaseID == vm.currentNode.TestId
          ).ExtendedField8,
          TrainType: this.testCaseModel.find(
            (x) => x.TestCaseID == vm.currentNode.TestId
          ).TrainType,

          selected: async (checkC, ExtendedField7, ExtendedField8) => {
            if (checkC) {
              this.testCaseModel.find(
                (x) => x.TestCaseID == vm.currentNode.TestId
              ).ExtendedField7 = ExtendedField7
              this.testCaseModel.find(
                (x) => x.TestCaseID == vm.currentNode.TestId
              ).ExtendedField8 = ExtendedField8
            }
          },
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    async Savesign() {
      let vm = this

      await GetTestRecodeDatas().then((res) => {
        this.TestRecodesOptions = res
      })

      try {
        window.$showDialog(TestStepRecodeSignDialog, {
          TestCaseID: vm.currentNode.TestId,
          TestRecodesDatas: vm.TestRecodesDatas,
          TestRecodeschecked: vm.TestRecodesDatas.map((x) => x.RecodeName),
          TestRecodesOptions: vm.TestRecodesOptions,

          selected: async (checkC, res) => {
            if (checkC) {
              this.testCaseModel.find(
                (x) => x.TestCaseID == this.currentNode.TestId
              ).TestRecodes = res
              this.TestRecodesDatas = res
            }
          },
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    RunTest() {
      var TestCaseID = -1
      var TestCaseGUID = ''
      var TestCaseName = ''
      var TestCaseType = 0
      var Remark = ''

      try {
        window.$showDialog(TestStepAddExecuteDialog, {
          TestCaseID: TestCaseID,
          TestCaseGUID: TestCaseGUID,
          TestCaseName: TestCaseName,
          TestCaseType: TestCaseType,
          Remark: Remark,

          selected: async (checkC) => {
            console.log('checkC', checkC)
            if (checkC) {
              this.GetData()
            }
          },
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    backtologin() {
      router.replace({ path: '/' }).then(() => {})
    },
    onBeforeClose(done) {
      done()
    },
    TestCase_AddNew() {
      this.TestCase_NewOrEdit(null)
    },
    TestCase_Edit() {
      if (this.currentNode == null) {
        this.$message.error('请先选择一行数据！')
        return
      }
      this.TestCase_NewOrEdit(this.currentNode)
    },
    TestCase_NewOrEdit(ActTestCase) {
      let vm = this
      try {
        var TestCaseID = -1
        var ExtendedField1 = ''
        var ExtendedField2 = ''
        var TestCaseGUID = ''
        var TestCaseName = ''
        var TrainType = 0
        var Remark = ''
        var checkC = false

        if (
          ActTestCase != null &&
          vm.testCaseModel.map((x) => x.TestCaseID).includes(ActTestCase.TestId)
        ) {
          var testCaseclass = vm.testCaseModel.find(
            (x) => x.TestCaseID == ActTestCase.TestId
          )

          if (testCaseclass != null) {
            TestCaseID = testCaseclass.TestCaseID
            ExtendedField1 = testCaseclass.ExtendedField1
            ExtendedField2 = testCaseclass.ExtendedField2
            TestCaseGUID = testCaseclass.TestCaseGUID
            TestCaseName = testCaseclass.TestCaseName
            TrainType = testCaseclass.TrainType
            Remark = testCaseclass.Remark
          }
        }

        window.$showDialog(TestCaseEditDialog, {
          TestCaseID: TestCaseID,
          ExtendedField1: ExtendedField1,
          ExtendedField2: ExtendedField2,
          TestCaseGUID: TestCaseGUID,
          TestCaseName: TestCaseName,
          TrainType: TrainType,
          Remark: Remark,
          Treedata: vm.Treedata,

          selected: async (checkC) => {
            if (checkC) {
              this.GetData()
            }
          },
        })

        if (checkC) {
          this.GetData()
        }
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    TestCase_Delete() {
      let vm = this
      try {
        if (this.currentNode == null) {
          this.$message.error('请先选择一行数据！')
          return
        }
        var TestCaseID = -1
        if (this.currentNode != null) {
          TestCaseID = this.currentNode.TestId
        }

        console.log('this.currentNode', this.currentNode)
        console.log('TestCaseID', TestCaseID)
        this.$confirm(`是否确定删除？`, '警告', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
        }).then(async () => {
          if (TestCaseID >= 0) {
            DeleteTestCase({
              TestCaseID: TestCaseID,
            })
              .then((res) => {
                this.GetData()
                if (res) {
                  vm.$message.success('删除成功！')
                } else {
                  vm.$message.error('删除失败！')
                }
              })
              .catch((err) => {
                console.error(err)
                vm.$message.error('删除失败！' + err)
              })
              .finally(() => {
                vm.$hideLoading()
              })
          }
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    TestStep_Delete() {
      let vm = this
      try {
        if (this.CurrentTestStep == null) {
          this.$message.error('请先选择一行数据！')
          return
        }
        var TestStepID = -1
        if (this.CurrentTestStep != null) {
          TestStepID = this.CurrentTestStep.TestStepID
        }

        console.log('this.CurrentTestStep', this.CurrentTestStep)
        this.$confirm(`是否确定删除？`, '警告', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
        }).then(async () => {
          if (TestStepID >= 0) {
            DeleteTestStep({
              TestStepID: TestStepID,
            })
              .then((res) => {
                GetTestStepDatas({ TestCaseID: this.currentNode.TestId }).then(
                  (res) => {
                    this.TestCommands = this.normalizeTestSteps(res)
                  }
                )
                if (res) {
                  vm.$message.success('删除成功！')
                } else {
                  vm.$message.error('删除失败！')
                }
              })
              .catch((err) => {
                console.error(err)
                vm.$message.error('删除失败！' + err)
              })
              .finally(() => {
                vm.$hideLoading()
              })
          }
        })
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
    },
    TestCase_Execute() {
      let vm = this
      try {
        if (this.currentNode == null) {
          this.$message.error('请先选择一行数据！')
          return
        }
        var TestCaseID = -1
        if (this.currentNode != null) {
          TestCaseID = this.currentNode.TestId
        }
        console.log("TestCaseID",TestCaseID)
          if (TestCaseID >= 0) {
            ExecuteTestCase({
              TestCaseID: TestCaseID,
            })
              .then((res) => {
                this.GetData()
                if (res) {
                  vm.$message.success('执行成功！')
                } else {
                  vm.$message.error('执行失败！')
                }
              })
              .catch((err) => {
                console.error(err)
                vm.$message.error('执行失败！' + err)
              })
              .finally(() => {
                vm.$hideLoading()
              })
          }
      } catch (error) {
        console.error(error)
        this.$message.error('执行失败！')
      }
    },


  },  
}
</script>
<style lang="scss" scoped>
.TestCasePage {
  height: 95vh;
  background: url('~@/assets/images/loginBG2.jpg') center center fixed no-repeat;
  background-size: cover;

  .title {
    font-size: 54px;
    font-weight: 600;
    color: #333333;
  }
}
.GroupViews {
  display: flex;
  overflow-x: auto;
  width: calc(100% - 40px);
  padding: 20px;
  height: 95vh;
}
.dialog-top1 {
  display: flex;
  overflow-x: auto;
}
.dialog-top2 {
  display: flex;
  overflow-x: auto;
}
.groupTestCaseListView1 {
  width: calc(30% - 20px) !important;
  flex: 3;
  display: flex;
  flex-direction: column;
  min-height: 200px;
  padding-left: 20px;
  padding-right: 20px;
  padding-bottom: 20px;
  border: 5px solid #e8eaec;
}
.groupTestCaseListView2 {
  width: calc(60% - 20px) !important;
  flex: 6;
  display: flex;
  flex-direction: column;
  min-height: 200px;
  padding-left: 20px;
  padding-right: 20px;
  padding-bottom: 20px;
  border: 5px solid #e8eaec;
}
.groupTestCaseListView3 {
  flex: 3;
  display: flex;
  flex-direction: column;
  min-height: 200px;
  padding-left: 20px;
  padding-right: 20px;
  padding-bottom: 20px;
  border: 5px solid #e8eaec;
}
.DetailsView {
  flex: 1;
  overflow: auto;
  position: relative;
  min-height: 200px;
}
.ChartView {
  overflow: auto;
  display: grid;
  width: calc(100% - 35px) !important;
  height: calc(100% - 35px);
  white-space: nowrap;
  padding-left: 15px;
  padding-top: 15px;
  border: 1px solid rgba(0, 0, 0, 0.247);
}


.step-tree-view {
  padding: 4px 0;
  font-size: 13px;
}

.step-tree-node {
  display: flex;
  align-items: center;
  padding: 6px 10px;
  border: 1px solid #e8e8e8;
  border-radius: 4px;
  margin-bottom: 3px;
  cursor: pointer;
  transition: all 0.15s;
  background: #fff;
  gap: 8px;
}
.step-tree-node:hover {
  border-color: #1890ff;
  background: #e6f7ff;
}
.step-tree-node.step-selected {
  border-color: #1890ff;
  box-shadow: 0 0 0 2px rgba(24, 144, 255, 0.2);
}

/* IF 閼哄倻鍋?*/
.step-tree-node.step-if {
  border-left: 4px solid #fadb14;
  background: linear-gradient(90deg, #fffbe6 0%, #fff 20%);
}
.step-tree-node.step-if:hover {
  border-color: #d4b106;
  background: #fff8cc;
}

/* FOR 閼哄倻鍋?*/
.step-tree-node.step-for {
  border-left: 4px solid #52c41a;
  background: linear-gradient(90deg, #f6ffed 0%, #fff 20%);
}
.step-tree-node.step-for:hover {
  border-color: #389e0d;
  background: #e8f8e0;
}

/* 濞ｅ崬瀹?閿涘牆鐡欏銉╊€冮敍?*/
.step-tree-node.depth-1 {
  margin-left: 24px;
}

/* 鐏炴洖绱?閹舵ê褰旈幐澶愭尦 */
.step-expand-btn {
  width: 16px;
  font-size: 10px;
  color: #999;
  cursor: pointer;
  flex-shrink: 0;
  text-align: center;
}

/* 鎼村繐褰?*/
.step-seq {
  font-size: 11px;
  color: #aaa;
  font-family: Consolas, monospace;
  width: 34px;
  text-align: center;
  flex-shrink: 0;
}

/* 濮濄儵顎冪猾璇茬€峰鐣岀彿 */
.step-type-badge {
  font-size: 11px;
  padding: 1px 6px;
  border-radius: 3px;
  flex-shrink: 0;
}
.badge-normal {
  background: #e6f7ff;
  color: #1890ff;
  border: 1px solid #91d5ff;
}
.badge-if {
  background: #fffbe6;
  color: #d48806;
  border: 1px solid #fadb14;
}
.badge-for {
  background: #f6ffed;
  color: #389e0d;
  border: 1px solid #52c41a;
}


.step-name {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: #333;
}
.step-value {
  font-size: 12px;
  color: #666;
  flex-shrink: 0;
}
.step-if-condition {
  font-size: 12px;
  color: #d48806;
  font-family: Consolas, monospace;
  flex-shrink: 0;
}
.step-for-desc {
  font-size: 12px;
  color: #389e0d;
  font-family: Consolas, monospace;
  flex-shrink: 0;
}
.step-remark {
  font-size: 11px;
  color: #999;
  flex-shrink: 0;
}
.step-actions {
  display: flex;
  align-items: center;
  gap: 2px;
  flex-shrink: 0;
  margin-left: auto;
}

.step-branch-area {
  margin-left: 12px;
  padding-left: 12px;
  border-left: 2px solid #e8e8e8;
  margin-bottom: 4px;
}

.branch-header {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  font-size: 12px;
  font-weight: 600;
  margin: 4px 0;
  border-radius: 3px;
}
.branch-then-header {
  color: #1890ff;
  background: #e6f7ff;
  border-left: 3px solid #1890ff;
}
.branch-else-header {
  color: #fa8c16;
  background: #fff7e6;
  border-left: 3px solid #fa8c16;
}
.branch-for-header {
  color: #52c41a;
  background: #f6ffed;
  border-left: 3px solid #52c41a;
}

.branch-children {
  padding-left: 8px;
}
.branch-empty {
  color: #ccc;
  font-size: 11px;
  padding: 4px 8px;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 50px 20px;
}

.drag-handle {
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: grab;
  color: #ccc;
  font-size: 13px;
  flex-shrink: 0;
  border-right: 1px solid #f0f0f0;
  margin-right: 2px;
  user-select: none;
  transition: color 0.15s;
}
.drag-handle:hover { color: #1890ff; }
.drag-handle:active { cursor: grabbing; }

.step-tree-node.dragging {
  opacity: 0.4;
}

.step-tree-node.drag-over-top {
  border-top: 2px solid #1890ff;
}
.step-tree-node.drag-over-bottom {
  border-bottom: 2px solid #1890ff;
}
.step-tree-node.drag-over-child {
  border: 2px dashed #1890ff;
  background: #f0f7ff;
}

</style>
