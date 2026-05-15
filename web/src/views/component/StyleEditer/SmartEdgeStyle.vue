<!-- 连线样式 -->
<template>
    <table cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="group1 unselectable" style="background-color: #eee;" colspan="10"
                    @click="() => { isShow = !isShow }">
                    <i class="el-icon-brush" />
                    连线样式
                    <i :class="{ 'el-icon-arrow-down': isShow, 'el-icon-arrow-up': !isShow }" style="float: right;" />
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px;">
                <td class="unselectable" colspan="3" style="border-bottom: 0;">线条</td>
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
                        <el-select placeholder="线条样式" style="width: 80px;" v-model="selectedEdgeType"
                            @change="handlerEdgeType">
                            <el-option v-for="(item, index) in edgeTypes" :key="index" :label="item"
                                :value="item"></el-option>
                        </el-select>
                    </div>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px;">
                <td class="unselectable" colspan="3">始端箭头</td>
                <td colspan="7" class="value">
                    <div class="stroke-size-input">
                        <el-select placeholder="箭头样式" style="display: block; float: left;width: 80px;"
                            v-model="selectedStartArrow" @change="handlerStartArrow">
                            <el-option v-for="(item, index) in Arrows" :key="index" :label="item" :value="item"></el-option>
                        </el-select>
                        <div class="stroke-size-input" style="display: block;float: left;margin-left: 6px;">
                            <el-input-number controls-position="right" :min="1" :max="10" data-unit="pt"
                                class="unit-el-input-number" v-model="startSize" @change="handlerStartSize" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px;">
                <td class="unselectable" colspan="3">末端箭头</td>
                <td colspan="7" class="value">
                    <div class="stroke-size-input">
                        <el-select placeholder="箭头样式" style="display: block; float: left;width: 80px;"
                            v-model="selectedEndArrow" @change="handlerEndArrow">
                            <el-option v-for="(item, index) in Arrows" :key="index" :label="item" :value="item"></el-option>
                        </el-select>
                        <div class="stroke-size-input" style="display: block;float: left;margin-left: 6px;">
                            <el-input-number controls-position="right" :min="1" :max="10" data-unit="pt"
                                class="unit-el-input-number" v-model="endSize" @change="handlerEndSize" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px;">
                <td class="unselectable" colspan="3">跨线</td>
                <td colspan="7" class="value">
                    <div class="stroke-size-input">
                        <el-select placeholder="跨线样式" style="display: block;float: left;width: 80px;"
                            v-model="selectedJumpStyle" @change="handlerJumpStyle">
                            <el-option v-for="(item, index) in JumpStyles" :key="index" :label="item"
                                :value="item"></el-option>
                        </el-select>
                        <div class="stroke-size-input" style="display: block;float: left;margin-left: 6px;">
                            <el-input-number controls-position="right" :min="1" :max="10" data-unit="pt"
                                aria-disabled="selectedJumpStyle == '无'" class="unit-el-input-number" v-model="jumpSize"
                                @change="handlerJumpSize" />
                        </div>
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
            strokeWidth: 1,
            strokeColor: '#666',
            strokeColors: ['#666', '#eee', '#ccc', '#222', '#000', '#fff'],
            selectedEdgeType: '实线',
            edgeTypes: ['实线', '虚线-1', '虚线-2'],
            selectedStartArrow: '无箭头',
            selectedEndArrow: '普通箭头',
            Arrows: ['普通箭头', '实心箭头', '菱形箭头', '无箭头'],
            selectedJumpStyle: '无',
            JumpStyles: ['无', '圆弧', '缝隙'],
            jumpSize: 6,
            startSize: 8,
            endSize: 8,
        }
    },
    watch: {
        currentSelectedDiagramLinks: {
            handler(val) {
                this.isShow = val?.length > 0
                if (this.isShow == false) {
                    return
                }
                let styles = this.$DiagramUtils.StyleSerialize(val[0].style)
                this.strokeColor = styles.strokeColor
                this.strokeWidth = styles.strokeWidth
            },
            deep: true,
        },
    },
    methods: {
        /* 修改线条颜色 */
        handlerStrokeColor() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'strokeColor', this.strokeColor)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改线条尺寸 */
        handlerStrokeWidth() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'strokeWidth', this.strokeWidth)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改线条样式 */
        handlerEdgeType() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedEdgeType == '实线') {
                    style = mxUtils.setStyle(style, 'dashed', null)
                    style = mxUtils.setStyle(style, 'dashPattern', null)
                }
                else if (this.selectedEdgeType == '虚线-1') {
                    style = mxUtils.setStyle(style, 'dashed', 1)
                }
                else if (this.selectedEdgeType == '虚线-2') {
                    style = mxUtils.setStyle(style, 'dashed', 1)
                    style = mxUtils.setStyle(style, 'dashPattern', '8 8')
                }
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改起点箭头样式 */
        handlerStartArrow() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedStartArrow == '普通箭头') {
                    style = mxUtils.setStyle(style, 'startFill', 0)
                    style = mxUtils.setStyle(style, 'startArrow', 'open')
                }
                else if (this.selectedStartArrow == '实心箭头') {
                    style = mxUtils.setStyle(style, 'startFill', 1)
                    style = mxUtils.setStyle(style, 'startArrow', 'blockThin')
                }
                else if (this.selectedStartArrow == '菱形箭头') {
                    style = mxUtils.setStyle(style, 'startFill', 1)
                    style = mxUtils.setStyle(style, 'startArrow', 'diamondThin')
                }
                else if (this.selectedStartArrow == '无箭头') {
                    style = mxUtils.setStyle(style, 'startFill', 0)
                    style = mxUtils.setStyle(style, 'startArrow', null)
                }
                style = mxUtils.setStyle(style, 'startSize', this.startSize)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改终点箭头样式 */
        handlerEndArrow() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedEndArrow == '普通箭头') {
                    style = mxUtils.setStyle(style, 'endFill', 0)
                    style = mxUtils.setStyle(style, 'endArrow', 'open')
                }
                else if (this.selectedEndArrow == '实心箭头') {
                    style = mxUtils.setStyle(style, 'endFill', 1)
                    style = mxUtils.setStyle(style, 'endArrow', 'blockThin')
                }
                else if (this.selectedEndArrow == '菱形箭头') {
                    style = mxUtils.setStyle(style, 'endFill', 1)
                    style = mxUtils.setStyle(style, 'endArrow', 'diamondThin')
                }
                else if (this.selectedEndArrow == '无箭头') {
                    style = mxUtils.setStyle(style, 'endFill', 0)
                    style = mxUtils.setStyle(style, 'endArrow', 'none')
                }
                style = mxUtils.setStyle(style, 'endSize', this.endSize)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改起点箭头尺寸 */
        handlerStartSize() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'startSize', this.startSize)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改终点箭头尺寸 */
        handlerEndSize() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'endSize', this.endSize)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改跨线样式 */
        handlerJumpStyle() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedJumpStyle == '无') {
                    style = mxUtils.setStyle(style, 'jumpStyle', null)
                }
                else if (this.selectedJumpStyle == '圆弧') {
                    style = mxUtils.setStyle(style, 'jumpStyle', 'arc')
                }
                else if (this.selectedJumpStyle == '缝隙') {
                    style = mxUtils.setStyle(style, 'jumpStyle', 'gap')
                }
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
        /* 修改跨线尺寸 */
        handlerJumpSize() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'jumpSize', this.jumpSize)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramLinks, resetStyle)
        },
    },
}
</script>
<style lang="scss" scoped>
@import 'table.scss';
@import 'index.scss';
</style>