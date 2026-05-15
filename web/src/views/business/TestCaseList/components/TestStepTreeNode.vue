<template>
  <div class="step-tree-item">
    <div
      class="step-tree-node"
      :class="nodeClass"
      @click="$emit('row-click', step)"
      @dragover="forwardDragOver"
      @dragleave="forwardDragLeave"
      @drop="forwardDrop"
    >
      <span
        class="drag-handle"
        draggable="true"
        @dragstart="forwardDragStart"
        @dragend="$emit('drag-end')"
        @click.stop
      >::</span>
      <span
        v-if="hasVisibleChildren"
        class="step-expand-btn"
        @click.stop="$emit('toggle-expand', step.TestStepID)"
      >
        {{ expanded ? '-' : '+' }}
      </span>
      <span v-else class="step-expand-btn step-expand-placeholder">+</span>

      <div class="step-main">
        <span class="step-seq">{{ step.DisplayOrder || step.TestCaseID }}</span>
        <span class="step-type-badge" :class="badgeClass">
          {{ getStepTypeLabel(step) }}
        </span>
        <span class="step-name">{{ getStepTitle(step) }}</span>
        <span v-if="isIfStep && step.IfCondition" class="step-if-condition">
          Condition: {{ step.IfCondition }}
        </span>
        <span v-if="isForStep" class="step-for-desc">
          {{ getForDesc(step) }}
        </span>
        <span v-if="step.Remark && !isIfStep && !isForStep" class="step-remark">
          {{ step.Remark }}
        </span>
      </div>

      <div class="step-meta">
        <span v-if="step.StepValue" class="step-value">Value: {{ step.StepValue }}</span>
        <span class="step-actions">
          <el-image
            class="step-icon"
            :src="step.url"
            @error="handleImageError"
          ></el-image>
          <el-image
            class="step-icon"
            :src="step.Logurl"
            @error="handleImageError"
          ></el-image>
        </span>
      </div>
    </div>

    <div v-if="expanded && isIfStep" class="step-branch-area">
      <div class="branch-header branch-then-header">
        <span>THEN</span>
        <el-button type="text" size="mini" @click="forwardAddChild(step.TestStepID, 'Execute', 'then')">Add</el-button>
      </div>
      <div class="branch-children">
        <TestStepTreeNode
          v-for="child in thenChildren"
          :key="child.TestStepID"
          :step="child"
          :current-test-step="currentTestStep"
          :expanded-map="expandedMap"
          :default-image-src="defaultImageSrc"
          :depth="depth + 1"
          @row-click="$emit('row-click', $event)"
          @drag-over="forwardNestedDragOver"
          @drag-leave="forwardNestedDragLeave"
          @drop="forwardNestedDrop"
          @drag-start="forwardNestedDragStart"
          @drag-end="$emit('drag-end')"
          @toggle-expand="$emit('toggle-expand', $event)"
          @add-child="forwardNestedAddChild"
        />
        <div v-if="thenChildren.length === 0" class="branch-empty">Empty branch</div>
      </div>
    </div>

    <div v-if="expanded && isForStep" class="step-branch-area">
      <div class="branch-header branch-for-header">
        <span>{{ getForDesc(step) }}</span>
        <el-button type="text" size="mini" @click="forwardAddChild(step.TestStepID, 'Execute', 'body')">Add</el-button>
      </div>
      <div class="branch-children">
        <TestStepTreeNode
          v-for="child in bodyChildren"
          :key="child.TestStepID"
          :step="child"
          :current-test-step="currentTestStep"
          :expanded-map="expandedMap"
          :default-image-src="defaultImageSrc"
          :depth="depth + 1"
          @row-click="$emit('row-click', $event)"
          @drag-over="forwardNestedDragOver"
          @drag-leave="forwardNestedDragLeave"
          @drop="forwardNestedDrop"
          @drag-start="forwardNestedDragStart"
          @drag-end="$emit('drag-end')"
          @toggle-expand="$emit('toggle-expand', $event)"
          @add-child="forwardNestedAddChild"
        />
        <div v-if="bodyChildren.length === 0" class="branch-empty">Empty branch</div>
      </div>
    </div>

    <div
      v-if="expanded && !isIfStep && !isForStep && normalChildren.length > 0"
      class="step-branch-area"
    >
      <div class="branch-header">
        <span>Children</span>
      </div>
      <div class="branch-children">
        <TestStepTreeNode
          v-for="child in normalChildren"
          :key="child.TestStepID"
          :step="child"
          :current-test-step="currentTestStep"
          :expanded-map="expandedMap"
          :default-image-src="defaultImageSrc"
          :depth="depth + 1"
          @row-click="$emit('row-click', $event)"
          @drag-over="forwardNestedDragOver"
          @drag-leave="forwardNestedDragLeave"
          @drop="forwardNestedDrop"
          @drag-start="forwardNestedDragStart"
          @drag-end="$emit('drag-end')"
          @toggle-expand="$emit('toggle-expand', $event)"
          @add-child="forwardNestedAddChild"
        />
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TestStepTreeNode',
  props: {
    step: { type: Object, required: true },
    currentTestStep: { type: [Object, Array], default: null },
    expandedMap: { type: Object, required: true },
    defaultImageSrc: { type: String, required: true },
    depth: { type: Number, default: 0 },
  },
  computed: {
    isIfStep() {
      return String(this.step.TestStepType || '').toUpperCase().includes('IF')
    },
    isForStep() {
      return String(this.step.TestStepType || '').toUpperCase().includes('FOR')
    },
    expanded() {
      return this.expandedMap[this.step.TestStepID] !== false
    },
    thenChildren() {
      return (this.step.children || []).filter(child => (child.BranchType || '') === 'then')
    },
    bodyChildren() {
      return (this.step.children || []).filter(child => (child.BranchType || '') === 'body')
    },
    normalChildren() {
      return (this.step.children || []).filter(child => !child.BranchType)
    },
    hasVisibleChildren() {
      return (this.step.children || []).length > 0
    },
    badgeClass() {
      if (this.isIfStep) return 'badge-if'
      if (this.isForStep) return 'badge-for'
      return 'badge-normal'
    },
    nodeClass() {
      return {
        'step-if': this.isIfStep,
        'step-for': this.isForStep,
        'step-selected': this.currentTestStep && this.currentTestStep.TestStepID === this.step.TestStepID,
        [`depth-${this.depth}`]: true,
      }
    },
  },
  methods: {
    getStepTitle(step) {
      if (this.isIfStep) return step.BindStepName || step.TestStepName || 'IF'
      if (this.isForStep) return step.TestStepName || 'FOR'
      return step.TestStepName || step.Remark || 'Untitled'
    },
    getStepTypeLabel(step) {
      if (step.sendReceiveType === 0) return 'TX'
      if (step.sendReceiveType === 1) return 'RX'
      if (this.isIfStep) return 'IF'
      if (this.isForStep) return 'FOR'
      return 'STEP'
    },
    getForDesc(step) {
      return `Loop ${Number(step.LoopCount || 1)}`
    },
    handleImageError(event) {
      event.target.src = this.defaultImageSrc
    },
    forwardDragOver(event) {
      this.$emit('drag-over', event, this.step)
    },
    forwardDragLeave(event) {
      this.$emit('drag-leave', event)
    },
    forwardDrop(event) {
      this.$emit('drop', event, this.step)
    },
    forwardDragStart(event) {
      this.$emit('drag-start', event, this.step)
    },
    forwardAddChild(parentStepId, stepType, branchType) {
      this.$emit('add-child', parentStepId, stepType, branchType)
    },
    forwardNestedDragOver(event, step) {
      this.$emit('drag-over', event, step)
    },
    forwardNestedDragLeave(event) {
      this.$emit('drag-leave', event)
    },
    forwardNestedDrop(event, step) {
      this.$emit('drop', event, step)
    },
    forwardNestedDragStart(event, step) {
      this.$emit('drag-start', event, step)
    },
    forwardNestedAddChild(parentStepId, stepType, branchType) {
      this.$emit('add-child', parentStepId, stepType, branchType)
    },
  },
}
</script>
