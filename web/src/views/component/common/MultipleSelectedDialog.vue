<template>
  <div class="addProjectDialog" style="padding: 50px">
    <el-dialog
      custom-class="DialogPopup"
      :title="title"
      width="70%"
      :visible.sync="dialogPopVisible"
      :before-close="onBeforeClose"
      :close-on-click-modal="false"
      :center="true"
    >
      <!-- 元素选择（多选） -->
      <div style="height: 50vh">
        <BaseTableSelectionMultiple
          :tableData="tableData"
          @selected="selectedchange"
          :columns="columns"
          :rowKey="rowKey"
          :childrenKey="childrenKey"
          :canSelection="true"
          :defaultSelectionRows="tableData"
        />
      </div>
      <!-- 取消、确认 -->
      <div class="dialog-footer">
        <span class="messageSpan">
          * 通过复选框取消选中项，然后点击“确认”按钮保存。
        </span>
        <div class="btnBox">
          <el-button @click="() => (dialogPopVisible = false)">取 消</el-button>
          <el-button :loading="loading" type="primary" @click="Save">
            确 定
          </el-button>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
  export default {
    name: 'MultipleSelectedDialog',
    data() {
      return {
        title: '当前选中项',
        loading: false,
        dialogPopVisible: true,
        tableData: [],
        columns: [],
        selected: [],
        rowKey: '',
        childrenKey: '',
      }
    },
    methods: {
      selectedchange(val) {
        this.selected = val
      },
      Save() {
        this.SaveAction(this.selected)
        this.dialogPopVisible = false
      },
      onBeforeClose(done) {
        done()
      },
    },
  }
</script>
