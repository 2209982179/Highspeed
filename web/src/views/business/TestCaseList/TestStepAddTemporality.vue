<template>
  <!-- 封装弹框 -->
  <el-dialog
    custom-class="DialogPopup"
    :title="title"
    :width="width"
    :visible.sync="dialogPopVisible"
    :before-close="onBeforeClose"
    :close-on-click-modal="false"
    :center="true"
  >
    <el-form @keyup.native.enter="sumbit" label-width="80px">
      <el-form-item label="名称:">
        <el-select v-model="TestCaseType" placeholder="请选择类型">
          <el-option
            v-for="(option, index) in CaseType"
            :key="'CaseType_' + index"
            :label="option.text"
            :value="option.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="监听步骤:">
        <el-input v-model="TestStep"></el-input>
      </el-form-item>
      <el-form-item label="数值:">
        <el-input v-model="TestCaseName"></el-input>
      </el-form-item>
      <el-form-item label="备注:">
        <el-input
          v-model="Remark"
          type="textarea"
          :rows="3"
          resize="none"
        ></el-input>
      </el-form-item>
    </el-form>
    <div slot="footer" class="dialog-footer" style="text-align: right">
      <el-button @click="Cancel">取 消</el-button>
      <el-button :loading="loading" type="primary" @click="Save">
        确 定
      </el-button>
    </div>
  </el-dialog>
</template>
<style>
.dialog-top {
  display: flex;
}
</style>
<script>
import {
  GetTestCaseList,
  CreateTestStep,
  UpdateTestStep,
} from '@/API/Admin'

export default {
  name: 'TestStepAdd',
  data() {
    return {
      title: '测试步骤编辑界面', // 窗体标题
      dialogPopVisible: false, // 窗体显示控制
      width: '500px',
      checkC: false,
      loading: false,
      radio: '1',
      CaseType: [
        { value: 0, text: 'IF' },
        { value: 1, text: 'FOR' },
      ],
      // dialog提交时的回调事件
      selected: null,
      TestCaseID: -1,
      TestStep:[],
      TestCaseGUID: '',
      TestCaseName: '',
      TestCaseType: 0,
      Remark: '',
    }
  },
  created() {
    this.GetData()
  },
  methods: {
    GetData() {},
    Save() {
      try {
        let vm = this
        vm.$showLoading()

        if (vm.TestCaseID == -1) {
          CreateTestStep({
            TestCaseID:vm.TestCaseID,
            TestCaseName: vm.TestCaseName,
            ExtendedField1: vm.ExtendedField1,
            ExtendedField2: vm.ExtendedField2,
            TrainType: vm.TrainType,
            Remark: vm.Remark,
          })
            .then((res) => {
              this.checkC = true

              if (res) {
                vm.$message.success('保存成功！')
              } else {
                vm.$message.error('保存失败！')
              }

              if (this.selected) {
                this.selected(
                  this.checkC,
                  this.TestCaseID,
                  this.TestCaseGUID,
                  this.TestCaseName,
                  this.TestCaseType,
                  this.Remark
                )
              }
            })
            .catch((err) => {
              console.error(err)
              vm.$message.error('保存失败：' + err)
            })
            .finally(() => {
              vm.$hideLoading()
            })
        } else {
          UpdateTestStep({
            TestCaseID: vm.TestCaseID,
            TestCaseName: vm.TestCaseName,
            ExtendedField1: vm.ExtendedField1,
            ExtendedField2: vm.ExtendedField2,
            TrainType: vm.TrainType,
            Remark: vm.Remark,
          })
            .then((res) => {
              this.checkC = true
              if (res) {
                vm.$message.success('保存成功！')
              } else {
                vm.$message.error('保存失败！')
              }

              if (this.selected) {
                this.selected(
                  this.checkC,
                  this.TestCaseID,
                  this.TestCaseGUID,
                  this.TestCaseName,
                  this.TestCaseType,
                  this.Remark
                )
              }
            })
            .catch((err) => {
              console.error(err)
              vm.$message.error('保存失败：' + err)
            })
            .finally(() => {
              vm.$hideLoading()
            })
        }
        this.dialogPopVisible = false
      } catch (error) {
        console.error(error)
        this.$message.error('错误')
      }
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
