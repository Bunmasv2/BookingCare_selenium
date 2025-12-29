export const formatTimeToLocale = (isoString) => {
  const date = new Date(isoString)
  let hours = date.getHours()
  const minutes = date.getMinutes().toString().padStart(2, '0')
  const ampm = hours >= 12 ? 'PM' : 'AM'
  hours = hours % 12 || 12

  return `${hours}:${minutes} ${ampm}`
}

export const formatDateToLocale = (dateString) => {
  const date = new Date(dateString)
    
  const day = date.getDate().toString().padStart(2, '0')
  const month = (date.getMonth() + 1).toString().padStart(2, '0')
  const year = date.getFullYear()
    
  return `${day}/${month}/${year}`
}
  
export const formatDateToISO = (dateString) => {
  const date = new Date(dateString)
    
  const year = date.getFullYear()
  const month = (date.getMonth() + 1).toString().padStart(2, '0')
  const day = date.getDate().toString().padStart(2, '0')
    
  return `${year}-${month}-${day}`
}

export const convertDateDMYtoYMD = (dateString) => {
  const [day, month, year] = dateString.split('/')
  return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
}
  
export const convertDateYMDtoDMY = (dateString) => {
  const [year, month, day] = dateString.split('-')
  return `${day}/${month}/${year}`
}
  
export const extractDateOnly = (dateTimeString) => {
  const date = new Date(dateTimeString)
    
  const day = date.getDate().toString().padStart(2, '0')
  const month = (date.getMonth() + 1).toString().padStart(2, '0')
  const year = date.getFullYear()
    
  return `${day}/${month}/${year}`
}
  
export const getTodayFormatted = () => {
  const today = new Date()
    
  const day = today.getDate().toString().padStart(2, '0')
  const month = (today.getMonth() + 1).toString().padStart(2, '0')
  const year = today.getFullYear()
    
  return `${day}/${month}/${year}`
}
  
export const parseDateString = (rawDate) => {
  // Try to detect format and parse accordingly
  let day, month, year
    
  if (rawDate.includes('/')) {
    // Format like DD/MM/YYYY
    [day, month, year] = rawDate.split('/')
  } else if (rawDate.includes('-')) {
    // Format like YYYY-MM-DD
    [year, month, day] = rawDate.split('-')
  } else {
    // Fallback to current date
    const today = new Date()
    day = today.getDate();
    month = today.getMonth() + 1
    year = today.getFullYear()
  }
    
  return {
    day: day.padStart(2, '0'),
    month: month.padStart(2, '0'),
    year
  }
}