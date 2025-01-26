// wwwroot/js/avatarUpload.js
window.handleAvatarUpload = async function (event) {
    const fileInput = event.target;
    const file = fileInput.files[0];

    console.log('Uploading avatar...');

    if (!file) {
        alert('No file selected.');
        return;
    }

    const formData = new FormData();
    formData.append('avatar', file);

    try {
        const response = await fetch('https://localhost:5002/api/FileUpload/upload-avatar', {
            method: 'POST',
            body: formData,
            headers: {
                'Accept': 'application/json'
            }
        });

        if (!response.ok) {
            const errorText = await response.text();
            alert('Upload failed: ' + errorText);
        } else {
            alert('Avatar uploaded successfully.');
            // Optionally, you can refresh the page or update the avatar image on the page.
        }
    } catch (error) {
        alert('Error uploading avatar: ' + error.message);
    }
}
