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
      <el-form-item label="执行车头:">
        <el-radio v-model="ExtendedField7" label="M1主控">M1主控</el-radio>
        <el-radio v-model="ExtendedField7" label="M2主控">M2主控</el-radio>
        <el-radio v-model="ExtendedField7" label="无需求">无需求</el-radio>
      </el-form-item>
      <el-form-item label="制动状态:">
        <el-input v-model="ExtendedField8"></el-input>
      </el-form-item>
      <el-form-item label="载荷状态:">
        <el-select v-model="TrainType" style="width: 100%">
          <el-option
            v-for="(option, index) in CaseType"
            :key="'CaseType_' + index"
            :label="option.text"
            :value="option.value"
          ></el-option>
        </el-select>
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
import { UpdatePreconditions } from '@/API/Admin'
export default {
  name: 'DialogPopupTestCase',
  data() {
    return {
      title: '用例前置条件界面', // 窗体标题
      dialogPopVisible: false, // 窗体显示控制
      width: '500px',
      checkC: false,
      loading: false,
      CaseType: [
        { value: 0, text: 'AW0' },
        { value: 1, text: 'AW1' },
        { value: 2, text: 'AW2' },
      ],
      ExtendedField7: '1',
      // dialog提交时的回调事件
      selected: null,
      TestCaseID: -1,
      ExtendedField8: '制动缓解',
      TrainType: '0',
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
      let vm = this
      this.checkC = true
      
      UpdatePreconditions({
        TestCaseID: this.TestCaseID,
        ExtendedField7: this.ExtendedField7,
        ExtendedField8: this.ExtendedField8,
      }).then((res) => {
        if (res == true) {
          this.$message.success('修改成功')
        }
        else
          this.$message.error('修改未成功保存！')
      }).catch((err) => {
          console.error(err)
          vm.$message.error('保存失败：' + err)
          this.dialogPopVisible = false
        })

      if (this.selected) {
        this.selected(this.checkC, this.ExtendedField7, this.ExtendedField8)
      }
      this.dialogPopVisible = false
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
