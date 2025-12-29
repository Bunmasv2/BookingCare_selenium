import React, { useState } from "react"
import axios from "../../Util/AxiosConfig"

const UploadDoctorImage = ({ doctorId }) => {
  const [file, setFile] = useState(null)

  const handleFileChange = (event) => {
    setFile(event.target.files[0])
  }

  const handleUpload = async () => {
    if (!file) {
      alert("Please select a file first.")
      return
    }

    const formData = new FormData()
    formData.append("file", file)
    formData.append("doctorId", 16)

    try {
        const response = await axios.post("/doctors/upload", formData, {
            headers: { 
                "Content-Type": "multipart/form-data" 
            },
        })
    } catch (error) {
      console.error("Upload failed:", error)
    }
  }

  return (
    <div>
      <input type="file" onChange={handleFileChange} />
      <button onClick={handleUpload}>Upload Image</button>
    </div>
  )
}

export default UploadDoctorImage
