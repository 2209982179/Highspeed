<template>
  <div class="admin-container">
    <el-container style="height: 100%">
      <!--左侧栏-->
      <el-aside width="auto" style="height: 100%" v-show="close != true">
        <!--左侧栏控件-->
      </el-aside>
      <el-container>
        <el-header style="padding: 0 10px">
        </el-header>
        <!--右侧栏-->
        <el-main>
          <!--左侧栏，这里就通过router-view来展示所需控件-->
          <div class="router-view">
            <router-view />
          </div>
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
  export default {
    name: 'admin',
    components: {},
    data() {
      return {
        close: true,
      }
    },
    mounted() {},
    methods: {
      closed(val) {
        this.close = val
      },
    },
  }
</script>
<style lang="scss" scoped>
  @import './admin.scss';
  .admin-container {
    height: 100%;
    width: 100%;
  }
  .el-main {
    padding: 20px;
    background-color: #f2f3f5;
  }
  .router-view {
    height: 100%;
  }
</style>
