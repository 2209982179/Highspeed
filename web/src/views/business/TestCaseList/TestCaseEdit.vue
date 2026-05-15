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
    <el-form label-width="100px">
      <el-form-item label="ID:">
        <el-input :disabled="true" v-model="TestCaseID"></el-input>
      </el-form-item>
      <el-form-item label="系统名称:"  required>
        <el-select
          v-model="ExtendedField1"
          filterable
          allow-create
          default-first-option
          placeholder="请选择系统名称"
          style="width: 100%"
          @change="DataSelect"
        >
          <el-option
            v-for="item in Systemoptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          >
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="试验名称:" required>
        <el-select
          v-model="ExtendedField2"
          filterableTest
          allow-create
          default-first-option
          placeholder="请选择试验名称"
          style="width: 100%"
          @change="DataSelect"
        >
          <el-option
            v-for="item in Testoptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          >
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="用例名称:" required>
        <el-input v-model="TestCaseName"></el-input>
      </el-form-item>
      <el-form-item label="列车类型:" required>
        <el-select
          v-model="TrainType"
          style="width: 100%"
          placeholder="请选择列车类型"
        >
          <el-option
            v-for="(option, index) in CaseType"
            :key="'CaseType_' + index"
            :label="option.text"
            :value="option.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="GUID:">
        <el-input :disabled="true" v-model="TestCaseGUID"></el-input>
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

<script>
import {
  AddNewTestCase,
  UpdateTestCase,
} from '@/API/Admin'

export default {
  name: 'DialogPopupTestCase',
  data() {
    return {
      title: '测试用例属性界面', // 窗体标题
      dialogPopVisible: false, // 窗体显示控制
      width: '500px',
      checkC: false,
      loading: false,
      CaseType: [
        { value: 0, text: 'AW0' },
        { value: 1, text: 'AW1' },
        { value: 2, text: 'AW2' },
      ],
      // dialog提交时的回调事件
      selected: null,
      TestCaseID: -1,
      TestCaseGUID: '',
      ExtendedField1: '',
      ExtendedField2: '',
      TestCaseName: '',
      TrainType: 0,
      Treedata: [],
      Systemoptions: [],
      Systemvalue: [],
      Testoptions: [],
      Testvalue: [],
      Remark: '',
    }
  },
  created() {
    this.GetData()
  },
  methods: {
    GetData() {
      let vm = this

      vm.Treedata.forEach((element) => {
        vm.Systemoptions.push({
          value: element.TestName,
          label: element.TestName,
        })
      })
    },
    Save() {
      try {
        let vm = this
        vm.$showLoading()

        if (vm.TestCaseID == -1) {
          AddNewTestCase({
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
          UpdateTestCase({
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
    DataSelect(item) {
      let vm = this
      vm.ExtendedField2 = null
      vm.Testoptions = []

      if (this.Treedata != null) {
        this.Treedata.find((x) => x.TestName == item).child.forEach(
          (element) => {
            vm.Testoptions.push({
              value: element.TestName,
              label: element.TestName,
            })
          }
        )
      }
    },
  },
}
</script>

<style>
.dialog-top {
  display: flex;
}
</style>
