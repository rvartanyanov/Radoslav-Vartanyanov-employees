import React, { useState } from 'react';
import axios from 'axios';

const UploadCSVComponent = () => {
  const [file, setFile] = useState(null);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const handleFileChange = (event) => setFile(event.target.files[0]);

  const handleUpload = async () => {
    if (!file) {
      setError('Please select a CSV file first.');
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    try {
      setError('');
      const { data } = await axios.post('https://localhost:7170/api/employee/FindLongestWorkingPair', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      setMessage(data.message || 'No message received');
    } catch (err) {
      setError(err.response?.data || 'An error occurred');
    }
  };

  return (
    <div>
      <h1>Upload Your CSV File Below</h1>
      <input type="file" accept=".csv" onChange={handleFileChange} />
      <button onClick={handleUpload}>Upload</button>
      {message && <div style={{ color: 'green', marginTop: '10px' }}>{message}</div>}
      {error && <div style={{ color: 'red', marginTop: '10px' }}>{error}</div>}
    </div>
  );
};

export default UploadCSVComponent;
