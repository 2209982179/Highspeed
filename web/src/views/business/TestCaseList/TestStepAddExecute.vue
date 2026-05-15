<template>
  <el-dialog
    custom-class="DialogPopup"
    :title="title"
    :width="width"
    :visible.sync="dialogPopVisible"
    :before-close="onBeforeClose"
    :close-on-click-modal="false"
    :center="true"
  >
    <el-form label-width="100px">
      <el-form-item label="步骤类型:">
        <el-select v-model="TestStepType" placeholder="请选择步骤类型" @change="onStepTypeChange">
          <el-option
            v-for="option in stepTypeList"
            :key="option.value"
            :label="option.text"
            :value="option.value"
          ></el-option>
        </el-select>
      </el-form-item>

      <template v-if="isBaseSignal">
        <el-form-item label="信号名称:">
          <el-select v-model="TestStepName" placeholder="请选择基础信号">
            <el-option
              v-for="option in stepNameOptions"
              :key="option.value"
              :label="option.text"
              :value="option.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="收发类型:">
          <el-radio v-model="radioType" label="Send">发送</el-radio>
          <el-radio v-model="radioType" label="Receive">接收</el-radio>
        </el-form-item>
        <el-form-item label="信号值:">
          <el-input v-model="StepValue"></el-input>
        </el-form-item>
      </template>

      <template v-if="isIfSignal">
        <el-form-item label="绑定步骤:">
          <el-select v-model="BindTestStepID" filterable placeholder="请选择已有基础信号">
            <el-option
              v-for="option in basicStepOptions"
              :key="option.value"
              :label="option.text"
              :value="option.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="比较符号:">
          <el-radio-group v-model="IfOperator">
            <el-radio-button
              v-for="operator in ifOperatorOptions"
              :key="operator"
              :label="operator"
            >{{ operator }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="比较值:">
          <el-input v-model="IfExpectedValue" placeholder="请输入比较值"></el-input>
        </el-form-item>
      </template>

      <template v-if="isForSignal">
        <el-form-item label="循环次数:">
          <el-input-number v-model="LoopCount" :min="1" :step="1"></el-input-number>
        </el-form-item>
      </template>

      <el-form-item label="备注:">
        <el-input v-model="Remark" type="textarea" :rows="3" resize="none"></el-input>
      </el-form-item>
    </el-form>

    <div slot="footer" class="dialog-footer" style="text-align: right">
      <el-button @click="Cancel">取消</el-button>
      <el-button :loading="loading" type="primary" @click="Save">确定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { CreateTestStep, UpdateTestStep } from '@/API/Admin'

export default {
  name: 'TestStepAdd',
  data() {
    return {
      title: '测试步骤编辑',
      dialogPopVisible: false,
      width: '520px',
      checkC: false,
      loading: false,
      selected: null,

      TestCaseID: -1,
      TestStepID: -1,
      PreviousID: -1,
      ParentID: 0,
      BranchType: null,
      TestStepType: 'Execute',
      TestStepName: '',
      StepValue: '',
      Remark: '',
      radioType: 'Send',

      BindTestStepID: null,
      IfOperator: '>',
      IfExpectedValue: '',
      LoopCount: 1,
      ifOperatorOptions: ['>', '>=', '==', '<', '<='],

      AvailableBasicSteps: [],
      stepTypeList: [
        { value: 'Execute', text: '基础信号' },
        { value: 'If', text: 'IF 判断' },
        { value: 'For', text: 'FOR 循环' },
      ],
      stepNameOptions: [
        { value: '牵引级位', text: '牵引级位' },
        { value: '速度', text: '速度' },
        { value: '制动缓解', text: '制动缓解' },
        { value: '惰行', text: '惰行' },
      ],
    }
  },
  computed: {
    isBaseSignal() {
      return this.TestStepType === 'Execute'
    },
    isIfSignal() {
      return this.TestStepType === 'If'
    },
    isForSignal() {
      return this.TestStepType === 'For'
    },
    basicStepOptions() {
      return (this.AvailableBasicSteps || [])
        .filter(step => this.normalizeStepType(step.TestStepType) === 'Execute' && step.TestStepID !== this.TestStepID)
        .map(step => ({
          value: step.TestStepID,
          text: `${step.TestStepName || '基础信号'} (${step.StepValue || '-'})`,
        }))
    },
  },
  methods: {
    normalizeStepType(stepType) {
      if (stepType === 1 || stepType === 'Execute') return 'Execute'
      if (stepType === 3 || stepType === 'If') return 'If'
      if (stepType === 4 || stepType === 'For') return 'For'
      return stepType || 'Execute'
    },
    normalizeSendReceiveType(sendReceiveType) {
      if (sendReceiveType === 0 || sendReceiveType === 'Send') return 'Send'
      if (sendReceiveType === 1 || sendReceiveType === 'Receive') return 'Receive'
      return 'Other'
    },
    onStepTypeChange(value) {
      if (value === 'If') {
        this.TestStepName = 'IF'
        this.StepValue = ''
        this.radioType = 'Other'
        this.IfOperator = this.IfOperator || '>'
        this.IfExpectedValue = this.IfExpectedValue || ''
        this.LoopCount = 1
      } else if (value === 'For') {
        this.TestStepName = 'FOR'
        this.StepValue = ''
        this.radioType = 'Other'
        this.BindTestStepID = null
        this.IfOperator = '>'
        this.IfExpectedValue = ''
      } else {
        this.BindTestStepID = null
        this.IfOperator = '>'
        this.IfExpectedValue = ''
        this.LoopCount = 1
        if (this.radioType === 'Other') {
          this.radioType = 'Send'
        }
      }
    },
    buildPayload() {
      return {
        TestCaseID: this.TestCaseID,
        TestStepID: this.TestStepID,
        PreviousID: Number(this.PreviousID || -1),
        ParentID: Number(this.ParentID || 0),
        BranchType: this.BranchType || '',
        TestStepType: this.normalizeStepType(this.TestStepType),
        TestStepName: this.isBaseSignal ? this.TestStepName : (this.isIfSignal ? 'IF' : 'FOR'),
        StepValue: this.isBaseSignal ? this.StepValue : '',
        sendReceiveType: this.isBaseSignal ? this.normalizeSendReceiveType(this.radioType) : 'Other',
        Remark: this.Remark || '',
        BindTestStepID: this.isIfSignal ? this.BindTestStepID : null,
        IfOperator: this.isIfSignal ? this.IfOperator : null,
        IfExpectedValue: this.isIfSignal ? this.IfExpectedValue : '',
        LoopCount: this.isForSignal ? Number(this.LoopCount || 1) : 1,
      }
    },
    validatePayload(payload) {
      if (payload.TestCaseID === -1) {
        this.$message.error('请先选择测试用例')
        return false
      }
      if (payload.TestStepType === 'Execute' && !payload.TestStepName) {
        this.$message.error('请选择基础信号')
        return false
      }
      if (payload.TestStepType === 'If' && !payload.BindTestStepID) {
        this.$message.error('请选择 IF 绑定的基础信号')
        return false
      }
      if (payload.TestStepType === 'If' && !payload.IfOperator) {
        this.$message.error('请选择 IF 比较符号')
        return false
      }
      if (payload.TestStepType === 'If' && !payload.IfExpectedValue) {
        this.$message.error('请输入 IF 比较值')
        return false
      }
      if (payload.TestStepType === 'For' && (!payload.LoopCount || payload.LoopCount < 1)) {
        this.$message.error('请输入正确的循环次数')
        return false
      }
      return true
    },
    Save() {
      const payload = this.buildPayload()
      if (!this.validatePayload(payload)) {
        return
      }

      this.loading = true
      this.$showLoading()

      const request = this.TestStepID === -1 ? CreateTestStep(payload) : UpdateTestStep(payload)
      request
        .then((res) => {
          this.checkC = !!res
          if (res) {
            this.$message.success('保存成功')
          } else {
            this.$message.error('保存失败')
          }

          if (this.selected) {
            this.selected(this.checkC)
          }

          if (res) {
            this.dialogPopVisible = false
          }
        })
        .catch((err) => {
          console.error(err)
          this.$message.error('保存失败: ' + err)
        })
        .finally(() => {
          this.loading = false
          this.$hideLoading()
        })
    },
    Cancel() {
      this.checkC = false
      this.dialogPopVisible = false
    },
    onBeforeClose(done) {
      done()
    },
  },
}
</script>
