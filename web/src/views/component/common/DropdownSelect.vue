<template>
    <el-select style="width: 100%;" :placeholder="placeholder" v-model="innerValue" filterable allow-create clearable
        :multiple="multiple" @change="handleChange">
        <slot name="options">
            <el-option v-for="item in dynamicOptions" :key="item[valueKey]" :label="item[labelKey]"
                :value="item[valueKey]">
            </el-option>
        </slot>
    </el-select>
</template>

<script>
export default {
    name: 'DropdownSelect',
    props: {
        value: [String, Number, Array],
        options: {
            type: Array,
            default: () => []
        },
        multiple: Boolean,
        // 支持自定义value/label字段名
        valueKey: {
            type: String,
            default: 'value'
        },
        labelKey: {
            type: String,
            default: 'label'
        },
        placeholder: {
            type: String,
            default: '请选择'
        }
    },
    data() {
        return {
            dynamicOptions: [...this.options]
        }
    },
    computed: {
        innerValue: {
            get() { return this.value },
            set(val) { this.$emit('input', val) }
        }
    },
    methods: {
        handleChange(val) {
            if (this.multiple) {
                val.forEach(v => this.checkAndAddOption(v));
            } else {
                this.checkAndAddOption(val);
            }
            this.$emit('change', val);
        },
        checkAndAddOption(val) {
            if (val && !this.dynamicOptions.some(opt => opt[this.valueKey] === val)) {
                const newOption = {
                    [this.labelKey]: val,
                    [this.valueKey]: val
                };
                this.dynamicOptions.push(newOption);
                this.$emit('option-add', newOption); // 通知父组件新增了选项
            }
        }
    },
    watch: {
        options(newVal) {
            this.dynamicOptions = [...newVal];
        }
    }
}
</script>