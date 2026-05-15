<!-- 基础表选择器，多选,分页，树 -->
<template>
    <div style="height: 100%;">
        <!-- <div class="table-container" style="height: calc(100% - 42px);"> -->
        <div class="table-container" :style="{ height: pagination ? 'calc(100% - 42px)' : '100%' }">
            <el-table ref="tableRef" height="100%" width="100%" :data="pagination ? pageData : tableData" :row-key="rowKey"
                v-loading="loading" :header-cell-style="{ 'background': '#F5F7FA' }" :tree-props="{ children: childrenKey }"
                @selection-change="handleSelectionChange" @select="handleRowSelect" @select-all="handleSelectAll" @row-click="rowClick"
                highlight-current-row  :default-expand-all="expandAll">
                <!-- 多选列 -->
                <el-table-column v-if="canSelection" type="selection" width="55" :selectable="checkSelectable">
                </el-table-column>
                <!-- 序号 -->
                <el-table-column type="index" label="序号" width="55" v-if="showIndex">
                    <template slot-scope="scope">
                        <span v-if="pagination">{{ (scope.$index + 1) + (pageIndex - 1) * pageSize }}</span>
                        <span v-else>
                            {{ (scope.$index + 1) }}
                        </span>
                    </template>
                </el-table-column>
                <!-- 其他列 -->
                <el-table-column v-for="(item, index) in columns" :key="index" :prop="item.prop" :label="item.label"
                    :width="item.width">
                    <template slot-scope="scope">
                        <span v-if="item.buttons?.length > 0">
                            <el-link v-for="(btn, btni) in item.buttons" :key="btni" :type="btn.type ?? 'primary'"
                                v-show="btn.show ? btn.show(scope.row) : true" @click="btn.click(scope.row)"
                                style="margin-right: 4px;">
                                {{ btn.label }}
                            </el-link>
                        </span>
                        <span v-else>
                            <!-- 提示文字与标题文字不一样时显示 -->
                            <el-tooltip effect="light"
                                v-if="item.tip && scope.row[item.tip] && scope.row[item.tip] != scope.row[item.prop]"
                                :content="scope.row[item.tip]">
                                <span>{{ scope.row[item.prop] }}</span>
                            </el-tooltip>
                            <span v-else>{{ scope.row[item.prop] }}</span>
                        </span>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div v-show="pagination" style="margin-top: 10px; text-align: right;height: 32px;">
            <el-row>
                <el-col :span="4">
                    <div style="text-align: left;height: 20px;line-height: 20px;padding-top:12px;">
                        <el-link type="primary" @click="showSelection">当前选中 {{ multipleSelection.length }} 行</el-link>
                    </div>
                </el-col>
                <el-col :span="20">
                    <el-pagination @size-change="pageSizeChange" @current-change="pageIndexChange"
                        :total="tableData.length ?? 0" :page-sizes="[10, 20, 30, 50, 100]" :page-size="pageSize"
                        :current-page="pageIndex" layout="total,sizes,prev,pager,next,jumper"></el-pagination>
                </el-col>
            </el-row>
        </div>
    </div>
</template>
<script>
import MultipleSelectedDialog from './MultipleSelectedDialog.vue'
export default {
    name: 'BaseTableSelectionMultiple',
    props: {
        // 所有数据行
        tableData: {
            type: Array,
            default: () => []
        },
        // 列配置
        columns: {
            type: Array,
            default: () => []
        },
        // 加载中
        loading: {
            type: Boolean,
            default: false
        },
        // 展开所有行
        expandAll: {
            type: Boolean,
            default: true
        },
        // 数据行主键属性
        rowKey: {
            type: String,
            default: 'RealID'
        },
        // 子项属性
        childrenKey: {
            type: String,
            default: null
        },
        // 默认选中数据行
        defaultSelectionRows: {
            type: Array,
            default: () => []
        },
        // 是否分页(树结构数据时，不支持分页)
        pagination: {
            type: Boolean,
            default: false
        },
        // 是否显示序号
        showIndex: {
            type: Boolean,
            default: false
        },
        // 是否提供复选框
        canSelection: {
            type: Boolean,
            default: true
        },
    },
    data() {
        return {
            multipleSelection: [],  //所有选中项（当前页 + 其他页）
            pageData: [],
            pageIndex: 1,
            pageSize: 10,
        }
    },
    watch: {
        tableData(val) {
            this.pageData = val.slice((this.pageIndex - 1) * this.pageSize, this.pageIndex * this.pageSize)
        },
        multipleSelection(val) {
            this.$emit('selected', val);
        },
    },
    mounted() {
        this.$nextTick(async () => {
            if (this.defaultSelectionRows?.length > 0) {
                this.toggleSelection(this.defaultSelectionRows, true);
            }
        })
    },
    methods: {
        rowClick(row){
            this.$emit('row-click', row)
        },
        // 检查此行是否可选
        checkSelectable(row) {
            return this.canSelection
        },
        // 选择元素（多选）
        handleSelectionChange(val) {
            this.multipleSelection = val
        },
        // 切换选择元素（多选）
        async toggleSelection(rows, flag) {
            let vm = this
            let childrenKey = this.childrenKey
            vm.$nextTick(async () => {
                if (rows) {
                    if (rows.length > 0) {
                        await rows.forEach(async (row) => {
                            if (vm.checkSelectable(row)) {
                                row['SelectedRow'] = flag
                                await vm.$refs.tableRef.toggleRowSelection(row, flag);
                                if (row[childrenKey] && row[childrenKey].length > 0) {
                                    await vm.toggleSelection(row[childrenKey], flag)
                                }
                            }
                        });
                    }
                } else {
                    let clearSelectedRow = (row) => {
                        row.SelectedRow = false
                        if (row[childrenKey]) {
                            row[childrenKey].forEach(item => {
                                clearSelectedRow(item)
                            })
                        }
                    }
                    if (vm.pagination) {
                        vm.pageData.forEach(element => {
                            clearSelectedRow(element)
                        });
                    }else{
                        vm.tableData.forEach(element => {
                            clearSelectedRow(element)
                        });
                    }
                    vm.$refs.tableRef.clearSelection();
                }
            })
        },
        // 行选中状态变化（多选）
        async handleRowSelect(selection, row) {
            let vm = this
            let childrenKey = this.childrenKey
            let index = selection.indexOf(row)
            row['SelectedRow'] = (index > -1)
            // 当子元素被取消选择时，父元素需要取消全选状态
            if (index <= -1) {
                if (row.ParentRow) {
                    row.ParentRow.SelectedRow = false
                    await vm.$refs.tableRef.toggleRowSelection(row.ParentRow, false);
                }
            }
            if (row[childrenKey] && row[childrenKey].length > 0) {
                // 批量选择
                if (index > -1) {
                    vm.toggleSelection(row[childrenKey], true);
                    return
                }
                if (index === -1) {
                    vm.toggleSelection(row[childrenKey], false);
                    return
                }
                if (index > -1) {
                    if (vm.pagination) {
                        let s = vm.pageData.filter(item => {
                            if (item.id === row.menuPid) {
                                return item;
                            }
                        });
                        vm.toggleSelection(s, true);
                        return
                    }else{
                        let s = vm.tableData.filter(item => {
                            if (item.id === row.menuPid) {
                                return item;
                            }
                        });
                        vm.toggleSelection(s, true);
                        return
                    }
                }
            }
        },
        // 全选/全不选（多选）
        handleSelectAll() {
            let vm = this;
            if (vm.$refs.tableRef.store.states.isAllSelected == true) {
                if (vm.pagination) {
                    vm.toggleSelection(vm.pageData, true);
                }else{
                    vm.toggleSelection(vm.tableData, true);
                }
            } else {
                vm.toggleSelection();
            }
        },
        pageSizeChange(val) {
            this.pageSize = val
            this.ResetMultipleSelection()
        },
        pageIndexChange(val) {
            // 切换页
            this.pageIndex = val
            this.ResetMultipleSelection()
        },
        ResetMultipleSelection() {
            this.pageData = this.tableData.slice((this.pageIndex - 1) * this.pageSize, this.pageIndex * this.pageSize)
            // 还原选中项
            this.toggleSelection(this.multipleSelection, true);
            this.$forceUpdate()
        },
        // 查看选中项
        showSelection() {
            let vm = this 
            let tableData = JSON.parse(JSON.stringify(this.multipleSelection))
            let rowKey = this.rowKey
            let childrenKey = this.childrenKey
            tableData.forEach(item => {
                if (item.children?.length > 0) {
                    delete item.children
                }
            });
            let columns = this.columns.filter(x => x.buttons?.length > 0 == false)
            let SaveAction = (selected) => {
                var selectedIds = selected.map(x => x[rowKey]);
                selected  = vm.multipleSelection.filter(x=> selectedIds.includes(x[rowKey]))
                // 先全部取消
                vm.toggleSelection();
                // 还原选中项
                vm.toggleSelection(selected, true);
            }
            this.$showDialog(
                MultipleSelectedDialog,
                {
                    defaultSelectionIds: this.multipleSelection.map(x => x[rowKey]),
                    columns,
                    tableData,
                    rowKey,
                    childrenKey,
                    SaveAction,
                },
                true
            )
        },
    }
}
</script>
<style lang="scss" scoped>
::v-deep .el-table__cell {
    padding: 8px 0;
}
</style>