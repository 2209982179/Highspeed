<!-- 图元样式 -->
<template>
    <table cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="group1 unselectable" style="background-color: #eee;" colspan="10"
                    @click="() => { isShow = !isShow }">
                    <i class="el-icon-brush" />
                    图元样式
                    <i :class="{ 'el-icon-arrow-down': isShow, 'el-icon-arrow-up': !isShow }" style="float: right;" />
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3">填充</td>
                <td colspan="7" class="value fill-color-picker">
                    <el-color-picker :predefine="fillColors" v-model="fillColor" @change="handlerFillColor" />
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px;">
                <td class="unselectable" colspan="3" style="border-bottom: 0;">边框</td>
                <td colspan="7" class="value" style="border-bottom: 0;">
                    <div style="display: block; float: left;">
                        <el-color-picker :predefine="strokeColors" v-model="strokeColor" @change="handlerStrokeColor" />
                    </div>
                    <div class="stroke-size-input" style="display: block;float: left;margin-left: 6px;">
                        <el-input-number controls-position="right" :min="1" :max="10" data-unit="pt"
                            class="unit-el-input-number" v-model="strokeWidth" @change="handlerStrokeWidth" />
                    </div>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 24px;">
                <td class="unselectable" colspan="3"></td>
                <td colspan="7" class="value" style="padding-bottom: 4px;">
                    <div class="stroke-size-input">
                        <el-select placeholder="边框样式" style="width: 80px;" v-model="selectedStrokeType"
                            @change="handlerStrokeType">
                            <el-option v-for="(item, index) in StrokeTypes" :key="index" :label="item"
                                :value="item"></el-option>
                        </el-select>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script>
import mxgraph from '@/mxgraph/graph'
const { mxUtils } = mxgraph
import { batchSetStyle } from './index.vue';

export default {
    props: {
        currentDiagram: null,
        currentSelectedDiagramLinks: null,
        currentSelectedDiagramObjects: null,
    },
    data() {
        return {
            isShow: true,
            fillColor: '#666',
            fillColors: ['#666', '#eee', '#ccc', '#222', '#000', '#fff'],
            strokeColor: '#666',
            strokeColors: ['#666', '#eee', '#ccc', '#222', '#000', '#fff'],
            selectedStrokeType: '实线',
            StrokeTypes: ['实线', '虚线-1', '虚线-2'],
            strokeWidth: 1,
        }
    },
    watch: {
        currentSelectedDiagramObjects: {
            handler(val) {
                this.isShow = val?.length > 0
                if (this.isShow == false) {
                    return
                }
                let styles = this.$DiagramUtils.StyleSerialize(val[0].style)
                this.fillColor = styles.fillColor
                this.strokeColor = styles.strokeColor
                this.strokeWidth = styles.strokeWidth
            },
            deep: true,
        },
    },
    methods: {
        /* 修改填充颜色 */
        handlerFillColor() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'fillColor', this.fillColor)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 修改线条颜色 */
        handlerStrokeColor() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'strokeColor', this.strokeColor)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 修改边框尺寸 */
        handlerStrokeWidth() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'strokeWidth', this.strokeWidth)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 修改边框样式 */
        handlerStrokeType() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedStrokeType == '实线') {
                    style = mxUtils.setStyle(style, 'dashed', null)
                    style = mxUtils.setStyle(style, 'dashPattern', null)
                }
                else if (this.selectedStrokeType == '虚线-1') {
                    style = mxUtils.setStyle(style, 'dashed', 1)
                }
                else if (this.selectedStrokeType == '虚线-2') {
                    style = mxUtils.setStyle(style, 'dashed', 1)
                    style = mxUtils.setStyle(style, 'dashPattern', '8 8')
                }
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
    },
}
</script>
<style lang="scss" scoped>
@import 'table.scss';
@import 'index.scss';
</style>