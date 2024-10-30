export async function apiGetRequest(path: string) {
    const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

    return fetch(`${apiBaseUrl}${path}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    })
        .then(async (response) => {
            if (!response.ok) {
                throw new Error(`Error fetching data: ${response.statusText}`);
            }
            return await response.json();
        })
        .catch((error) => {
            console.error('Error fetching data:', error);
            return null;
        });
}