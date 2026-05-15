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
      <template>
        <el-checkbox
          :indeterminate="isIndeterminate"
          v-model="checkAll"
          @change="handleCheckAllChange"
          >全选</el-checkbox
        >
        <div style="margin: 15px 0; margin-top: 20px"></div>
        <el-checkbox-group
          size="medium"
          v-model="TestRecodeschecked"
          @change="handleCheckedCitiesChange"
        >
          <el-row>
            <el-col
              v-for="city in TestRecodesOptions.map((x) => x.RecodeName)"
              :key="city"
              :span="6"
            >
              <el-checkbox style="margin: 5px" :label="city"></el-checkbox>
            </el-col>
          </el-row>
        </el-checkbox-group>
      </template>
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
  UpdateTestRecodeDatas,
  GetOneTestRecodeDatas,
  UpdateTestCase,
  DeleteTestCase,
} from '@/API/Admin'
export default {
  name: 'DialogPopupTestCase',
  data() {
    return {
      title: '记录信号选择界面', // 窗体标题
      dialogPopVisible: false, // 窗体显示控制
      width: '800px',
      checkC: false,
      loading: false,
      radio: '1',
      checkAll: false,
      TestRecodeschecked: [],
      TestRecodesOptions: [],
      TestRecodesDatas: [],
      // dialog提交时的回调事件
      selected: null,
      TestCaseID: -1,
      isIndeterminate: true,
    }
  },
  created() {
    this.GetData()
  },
  methods: {
    GetData() {},
    Save() {
      let vm = this
      UpdateTestRecodeDatas(
        { TestCaseID: vm.TestCaseID },
        vm.TestRecodeschecked
      )
        .then((res) => {
          if (res == true) {
            this.$message.success('修改成功')
            GetOneTestRecodeDatas(
              { TestCaseID: vm.TestCaseID },
              vm.TestRecodesOptions
            ).then((res) => {
              vm.TestRecodesDatas = res

              this.checkC = true
              if (this.selected) {
                this.selected(this.checkC, res)
              }
              console.log("vm.TestRecodesDatas1111",res)
              console.log("vm.TestRecodesDatas11112",vm.TestRecodesDatas)
            })
          } else this.$message.error('修改未成功保存！')

          this.dialogPopVisible = false
        })
        .catch((err) => {
          console.error(err)
          vm.$message.error('保存失败：' + err)
          this.dialogPopVisible = false
        })
    },
    Cancel() {
      this.checkC = false
      this.dialogPopVisible = false
    },
    onBeforeClose(done) {
      done()
    },
    handleCheckAllChange(val) {
      this.TestRecodeschecked = val
        ? this.TestRecodesOptions.map((x) => x.RecodeName)
        : []
      this.isIndeterminate = false
    },
    handleCheckedCitiesChange(value) {
      let checkedCount = value.length
      this.checkAll =
        checkedCount === this.TestRecodesOptions.map((x) => x.RecodeName).length
      this.isIndeterminate =
        checkedCount > 0 &&
        checkedCount < this.TestRecodesOptions.map((x) => x.RecodeName).length
    },
  },
}
</script>
