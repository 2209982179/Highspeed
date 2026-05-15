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
    <label
      style="
        padding: 10px;
        width: max-content;
        line-height: 24px;
        color: rgb(168, 85, 7);
      "
    >
      <pre> {{ TipMessageShow }}</pre>
    </label>
    <div slot="footer" class="dialog-footer" style="text-align: right">
      <el-button class="confirm" type="primary" @click="OK">确 定</el-button>
    </div>
  </el-dialog>
</template>
<script>
  export default {
    name: 'MTBFPredictionCUItemsAdd_DialogPopup',
    data() {
      return {
        title: '提示', // 窗体标题
        dialogPopVisible: false, // 窗体显示控制
        width: '450px',
        parent: {},
        TipMessageShow: '',
      }
    },
    created() {
      this.GetData()
    },
    methods: {
      GetData() {},
      OK() {
        this.dialogPopVisible = false
      },
      onBeforeClose(done) {
        done()
      },
    },
  }
</script>
