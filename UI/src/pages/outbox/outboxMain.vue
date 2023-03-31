<template>
  <q-table ref="tableRef" row-key="_id" :rows="rows" :columns="columns" v-model:pagination="pagination" :loading="loading"
    :filter="filter" binary-state-sort @request="onRequest" dense>
    <template v-slot:top>
      <q-space />
      <q-input dense debounce="300" placeholder="搜索" color="primary" v-model="filter">
        <template v-slot:append>
          <q-icon name="search" />
        </template>
      </q-input>
    </template>

    <template v-slot:body-cell-index="props">
      <q-td :props="props">
        {{ props.rowIndex + 1 }}
      </q-td>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import userQTable from '../../compositions/useQTable'

const columns = [

  { name: 'calories', align: 'center', label: 'Calories', field: 'calories', sortable: true },
  { name: 'fat', label: 'Fat (g)', field: 'fat', sortable: true },
  { name: 'carbs', label: 'Carbs (g)', field: 'carbs', sortable: true },
  { name: 'protein', label: 'Protein (g)', field: 'protein', sortable: true },
  { name: 'sodium', label: 'Sodium (mg)', field: 'sodium', sortable: true }
]

const { filter, pagination, loading, rows, onRequest } = userQTable()
</script>

<style lang="scss" scoped></style>
