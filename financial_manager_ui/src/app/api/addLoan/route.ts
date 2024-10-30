export async function apiPostRequest(path: string, body: any) {
    const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

    return fetch(`${apiBaseUrl}${path}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(body),
    })
        .then(async (response) => {
            if (!response.ok) {
                throw new Error(`Error adding loan: ${response.statusText}`);
            }
            return await response.json();
        })
        .catch((error) => {
            console.error('Error adding loan:', error);
            return null;
        });
}