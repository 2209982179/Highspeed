<template>
  <div>
    <el-table border :data="data" style="width: 100%">
      <el-table-column
        v-for="(column, index) in columns"
        :key="index"
        :label="column.label"
        :prop="column.prop"
        header-align="center"
        align="center"
      >
        <template slot-scope="scope">
          <el-input
            v-if="scope.row['type'] == 'input'"
            placeholder="请输入内容"
            v-model="scope.row[column.prop]"
            clearable
          ></el-input>
          <el-select
            v-model="scope.row[column.prop]"
            placeholder="请选择"
            v-else-if="scope.row['type'] == 'select'"
          >
            <el-option
              v-for="item in options"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>
          <span v-else>{{ scope.row[column.prop] }}</span>
        </template>
      </el-table-column>
      <el-table-column
        v-if="withDelete"
        label="操作"
        align="center"
        header-align="center"
      >
        <template slot-scope="scope">
          <el-button
            size="mini"
            type="danger"
            @click="handleDelete(scope.$index)"
          >
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
<script>
  export default {
    name: 'ElasticTable',
    props: {
      columns: {
        type: Array,
      },
      data: {
        type: Array,
      },
      withDelete: {
        type: Boolean,
      },
    },
    data() {
      return {}
    },
    mounted() {},
    methods: {
      handleDelete(index) {
        this.data.splice(index, 1)
      },
    },
  }
</script>
