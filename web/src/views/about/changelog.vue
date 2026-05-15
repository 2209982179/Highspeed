<template>
  <div id="changelog" class="changelog">
    <el-dialog
      custom-class="DialogPopup"
      :title="title"
      :width="width"
      :visible.sync="dialogPopVisible"
      :before-close="onBeforeClose"
      :close-on-click-modal="false"
      :center="true"
    >
      <div class="header">
        <h1>软件版本【V1.0.0】</h1>
      </div>
      <h1>更新日志</h1>
      <div class="changelog-container">
        <div
          v-for="entry in changelog"
          :key="entry.version"
          class="changelog-entry"
        >
          <h2>版本号：{{ entry.version }}</h2>
          <div style="margin: 10px 0">
            <h2>发布日期：2025-08-25 {{ entry.date }}</h2>
          </div>
          <h2>内容详情：</h2>
          <div class="changelogUl">
            <ul style="margin-top: 2px">
              <li v-for="(item, index) in entry.changes" :key="item">
                {{ index + 1 }}：{{ item }}
              </li>
            </ul>
          </div>
          <el-divider></el-divider>
        </div>
      </div>
      <div slot="footer" class="dialog-footer" style="text-align: right">
        <el-button :loading="loading" type="primary" @click="Save">
          确 定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
  export default {
    name: 'changelog',
    data() {
      return {
        loading: false,
        title: '更新日志', // 窗体标题
        dialogPopVisible: false, // 窗体显示控制
        width: '1000px',
        changelog: [
          {
            version: 'V1.0.0',
            date: '2025-07-14',
            changes: ['【功能新增】初版发布1.0；'],
          },
        ],
      }
    },
    methods: {
      Save() { 
        this.dialogPopVisible = false
      },
      Cancel() {
        this.dialogPopVisible = false
      },
      onBeforeClose(done) {
        done()
      },
    },
  }
</script>

<style scoped>
  .changelog-entry {
    width: 100%;
    margin: 20px auto;
  }

  .changelog {
    padding: 20px;
    height: 100%;
    overflow-y: auto;
    margin: 0 auto;
    width: 95%;
    font-size: 20px !important;
    font-family: Arial, sans-serif;
  }

  ul li {
    line-height: 30px;
    list-style: none;
    font-size: 16px;
  }

  h1 {
    font-weight: bold;
    font-size: 24px;
  }

  h2 {
    font-size: 20px;
  }

  .header {
    text-align: center;
    width: 40%;
    margin: 20px auto;
  }
</style>
