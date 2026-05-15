<template>
    <div class="BaseCheckBoxGroup">
        <el-row v-if="canSelectAll && datas?.length > 1" style="height: 30px;">
            <el-col :span="24"><el-checkbox v-model="checkAll" @change="handleCheckAllChange"
                    :indeterminate="isIndeterminate">全部显示</el-checkbox></el-col>
        </el-row>
        <el-row v-if="datas?.length > 0">
            <el-col :span="12" v-for="item, index in datas" :key="index" style="height: 30px;">
                <el-tooltip :content="item.Stereotype" effect="light" placement="right">
                    <el-checkbox :label="item.Stereotype" v-model="item[checkedKey]"
                        :indeterminate="item[checkedKey] == null" @change="handleCheckChange(item)">{{
                            item.Alias ?? item.Stereotype }}</el-checkbox>
                </el-tooltip>
            </el-col>
        </el-row>
        <el-row v-else>
            {{ noDataMsg }}
        </el-row>
    </div>
</template>

<script>
export default {
    name: 'BaseCheckBoxGroup',
    props: {
        datas: {
            type: Array,
            default: () => []
        },
        checkedKey: {
            type: String,
            default: 'checked'
        },
        noDataMsg: {
            type: String,
            default: '没有数据'
        },
        handleSave: {
            type: Function,
            default: () => { }
        },
        canSelectAll: {
            type: Boolean,
            default: true
        },
    },
    data() {
        return {
            checkAll: false,
            checkNone: false,
            isIndeterminate: false,
        };
    },
    watch: {
        // 视图切换时，清空提示信息
        datas() {
            this.updateCheckAll()
        },
    },
    methods: {
        handleCheckAllChange: function (val) {
            this.isIndeterminate = false
            this.checkAll = val
            this.datas.forEach(element => {
                element[this.checkedKey] = val
            });
            // 修改全部类型
            this.$emit('handleSave', val, this.datas.map(x => x.Stereotype))
        },
        handleCheckChange: function (item) {
            item.isIndeterminate = false
            this.$emit('handleSave', item[this.checkedKey], [item.Stereotype])
            // 更新全选状态
            this.updateCheckAll()
        },
        updateCheckAll() {
            // 全部显示
            if (this.datas.every(x => x[this.checkedKey] == true)) {
                this.isIndeterminate = false
                this.checkAll = true
            }
            // 全部隐藏
            else if (this.datas.every(x => x[this.checkedKey] == false)) {
                this.isIndeterminate = false
                this.checkAll = false
            }
            // 有显示也有隐藏
            else {
                this.isIndeterminate = true
                this.checkAll = false
            }
        },
    }
};
</script>

<style>
.BaseCheckBoxGroup .el-checkbox__label {
    height: 24px !important;
    line-height: 24px !important;
}
</style>
