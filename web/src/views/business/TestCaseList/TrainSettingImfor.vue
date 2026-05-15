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
    <el-form @keyup.native.enter="sumbit" label-width="280px">
      <el-form-item label="编组型式:">
        <el-input v-model="FormationType"></el-input>
      </el-form-item>
      <el-form-item label="最高运行速度:">
        <el-input v-model="MaximumOperatingSpeed"></el-input>
      </el-form-item>
      <el-form-item label="平均旅行速度:">
        <el-input v-model="AverageTravelSpeed"></el-input>
      </el-form-item>
      <el-form-item label="平均站停时间:">
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="AverageDwellTime"
        ></el-input>
      </el-form-item>
      <el-form-item
        label="在满载条件下，在平直干燥轨道上，车轮半磨耗状态，额定电压时，平均加速度:"
      >
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="AverageAcceleration"
        ></el-input>
      </el-form-item>
      <el-form-item
        label="在满载条件下，在平直干燥轨道上，三车轮半磨耗状态，列车在最高运行速度80km/h，从给制动指令到停车时，平均减速度:"
      >
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="AverageDeceleration"
        ></el-input>
      </el-form-item>
      <el-form-item label="列车纵向冲击率:">
        <el-input v-model="TrainImpactRate"></el-input>
      </el-form-item>
      <el-form-item label="列车故障状态下运行能力要求:">
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="RequirementsFaultStatus"
        ></el-input>
      </el-form-item>
      <el-form-item label="列车坡道救援能力要求:">
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="RequirementsRampRescue"
        ></el-input>
      </el-form-item>
      <el-form-item label="车内噪声水平要求:">
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="RequirementsNoiseLevelIn"
        ></el-input>
      </el-form-item>
      <el-form-item label="车外噪声水平要求:">
        <el-input
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 4 }"
          v-model="RequirementsNoiseLevelOut"
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
import { UpdateTrainDatas } from '@/API/Admin'
export default {
  name: 'DialogTrainSetting',
  data() {
    return {
      title: '列车属性界面', // 窗体标题
      dialogPopVisible: false, // 窗体显示控制
      width: '1000px',
      checkC: false,
      loading: false,
      radio: '1',
      // dialog提交时的回调事件
      selected: null,
      TestCaseID: -1,
      TestCaseGUID: '',
      TrainName:'',
      TrainSettingDatas: [],
      FormationType: '',
      MaximumOperatingSpeed: '',
      AverageTravelSpeed: '',
      AverageDwellTime: '',
      AverageAcceleration: '',
      AverageDeceleration: '',
      TrainImpactRate: '',
      RequirementsFaultStatus: '',
      RequirementsRampRescue: '',
      RequirementsNoiseLevelIn: '',
      RequirementsNoiseLevelOut: '',
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
      UpdateTrainDatas({
        TrainName: vm.TrainName,
        FormationType: vm.FormationType,
        VehicleLoad: vm.VehicleLoad,
        MaximumOperatingSpeed: vm.MaximumOperatingSpeed,
        AverageTravelSpeed:vm.AverageTravelSpeed,
        AverageDwellTime:vm.AverageDwellTime,
        AverageAcceleration:vm.AverageAcceleration,
        AverageDeceleration:vm.AverageDeceleration,
        TrainImpactRate:vm.TrainImpactRate,
        RequirementsFaultStatus:vm.RequirementsFaultStatus,
        RequirementsRampRescue:vm.RequirementsRampRescue,
        RequirementsNoiseLevelIn:vm.RequirementsNoiseLevelIn,
        RequirementsNoiseLevelOut:vm.RequirementsNoiseLevelOut,
      }).then((res) => {})

      this.checkC = true
      if (this.selected) {
        this.selected(this.checkC, this.TestCaseID)
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
