// qTable 初始化
import { ref, onMounted } from 'vue'
import { asyncComputed } from '@vueuse/core'

export default function useQTable(initParams = {}) {
  const pagination = ref(
    Object.assign(
      {
        sortBy: '_id',
        descending: true,
        page: 1,
        rowsPerPage: 15,
        rowsNumber: 0
      },
      initParams.pagination || {}
    )
  )
  const loading = ref(false)
  const filter = ref('')
  const rows = ref([])

  const { countApi, dataApi } = initParams

  const rowsNumber = asyncComputed(async() => {
    if (!countApi) return 0

    const countRes = await countApi(filter)
    return countRes.data
  }, null)

  async function onRequest(props) {
    const { page, rowsPerPage, sortBy, descending } = props.pagination
    const filter = props.filter

    loading.value = true

    // emulate server

    // update rowsCount with appropriate value
    pagination.value.rowsNumber = rowsNumber

    if (rowsNumber.value === 0) {
      loading.value = false
      return
    }

    // get all rows if "All" (0) is selected
    const fetchCount =
      rowsPerPage === 0 ? pagination.value.rowsNumber : rowsPerPage

    // calculate starting row of data
    const startRow = (page - 1) * rowsPerPage

    // fetch data from "server"
    const returnedData = await dataApi(
      startRow,
      fetchCount,
      filter,
      sortBy,
      descending
    )

    // clear out existing data and add new
    rows.value.splice(0, rows.value.length, ...returnedData)

    // don't forget to update local pagination object
    pagination.value.page = page
    pagination.value.rowsPerPage = rowsPerPage
    pagination.value.sortBy = sortBy
    pagination.value.descending = descending

    // ...and turn of loading indicator
    loading.value = false
  }

  onMounted(async() => {
    // get initial data from server (1st page)
    await onRequest({ pagination: pagination.value })
  })

  return {
    filter,
    pagination,
    rowsNumber,
    rows,
    loading,
    onRequest
  }
}
